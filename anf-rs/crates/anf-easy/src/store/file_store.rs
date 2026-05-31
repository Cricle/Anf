use async_trait::async_trait;
use bytes::Bytes;
use std::path::{Path, PathBuf};
use std::sync::Arc;
use tokio::fs;

use crate::saver::{ComicDownloadContext, ComicSaver};
use crate::store::address_name::{
    sanitize_filename, AddressToFileNameProvider, Md5AddressProvider,
};
use crate::store::lru_cache::LruCacher;
use anf_core::url_helper::fast_get_host;

/// FileStoreService: stores comic images on disk
/// Matches C# FileStoreService (IStoreService + IComicSaver)
pub struct FileStoreService {
    folder: PathBuf,
    address_provider: Arc<dyn AddressToFileNameProvider>,
    file_cache: LruCacher<String, PathBuf>,
    domain_cache: LruCacher<String, PathBuf>,
}

impl FileStoreService {
    pub const DEFAULT_FOLDER_NAME: &'static str = "Stores";
    pub const DEFAULT_CACHE_SIZE: usize = 50;

    pub fn new(folder: PathBuf, address_provider: Arc<dyn AddressToFileNameProvider>) -> Self {
        std::fs::create_dir_all(&folder).ok();
        Self {
            folder,
            address_provider,
            file_cache: LruCacher::new(Self::DEFAULT_CACHE_SIZE),
            domain_cache: LruCacher::new(Self::DEFAULT_CACHE_SIZE),
        }
    }

    pub fn from_default(
        base_path: &str,
        address_provider: Arc<dyn AddressToFileNameProvider>,
    ) -> Self {
        let folder = Path::new(base_path).join(Self::DEFAULT_FOLDER_NAME);
        Self::new(folder, address_provider)
    }

    pub fn from_md5_default(base_path: &str) -> Self {
        Self::from_default(base_path, Arc::new(Md5AddressProvider))
    }

    fn get_file_name(&self, address: &str) -> String {
        self.address_provider.convert(address)
    }

    fn ensure_domain_folder(&self, address: &str) -> PathBuf {
        let host = fast_get_host(address);
        let host = sanitize_filename(host);
        self.domain_cache.get_or_insert_with(host.clone(), || {
            let path = self.folder.join(&host);
            std::fs::create_dir_all(&path).ok();
            path
        })
    }

    fn get_file_path(&self, address: &str) -> PathBuf {
        let key = self.get_file_name(address);
        if let Some(path) = self.file_cache.get(&key) {
            return path;
        }
        let domain_folder = self.ensure_domain_folder(address);
        let path = domain_folder.join(&key);
        self.file_cache.put(key, path.clone());
        path
    }

    pub async fn exists(&self, address: &str) -> bool {
        let path = self.get_file_path(address);
        path.exists()
    }

    pub fn get_path(&self, address: &str) -> PathBuf {
        self.get_file_path(address)
    }

    pub async fn save(&self, address: &str, data: &[u8]) -> anf_core::Result<PathBuf> {
        let path = self.get_file_path(address);
        if let Some(parent) = path.parent() {
            fs::create_dir_all(parent).await?;
        }
        fs::write(&path, data).await?;
        Ok(path)
    }

    pub async fn get_stream(&self, address: &str) -> Option<Bytes> {
        let path = self.get_file_path(address);
        if path.exists() {
            fs::read(&path).await.ok().map(Bytes::from)
        } else {
            None
        }
    }
}

#[async_trait]
impl ComicSaver for FileStoreService {
    fn need_to_save(&self, ctx: &ComicDownloadContext) -> bool {
        let path = self.get_file_path(&ctx.page.target_url);
        !path.exists()
    }

    async fn save(&self, ctx: &ComicDownloadContext) -> anf_core::Result<()> {
        if let Some(ref data) = ctx.source_data {
            self.save(&ctx.page.target_url, data).await?;
        }
        Ok(())
    }
}
