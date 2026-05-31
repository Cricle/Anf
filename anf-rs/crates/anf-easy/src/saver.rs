use async_trait::async_trait;
use bytes::Bytes;
use std::future::Future;
use std::pin::Pin;

use anf_core::{ComicChapter, ComicEntity, ComicPage};

/// DownloadContext: passed to saver during download
#[derive(Debug, Clone)]
pub struct ComicDownloadContext {
    pub entity: ComicEntity,
    pub chapter: ComicChapter,
    pub page: ComicPage,
    pub source_data: Option<Bytes>,
}

/// ComicSaver trait
#[async_trait]
pub trait ComicSaver: Send + Sync {
    fn need_to_save(&self, ctx: &ComicDownloadContext) -> bool;
    async fn save(&self, ctx: &ComicDownloadContext) -> anf_core::Result<()>;
}

/// DelegateComicSaver: closures-based saver
pub struct DelegateComicSaver<F, G>
where
    F: Fn(&ComicDownloadContext) -> bool + Send + Sync,
    G: for<'a> Fn(
            &'a ComicDownloadContext,
        ) -> Pin<Box<dyn Future<Output = anf_core::Result<()>> + Send + 'a>>
        + Send
        + Sync,
{
    need_to_save_fn: F,
    save_fn: G,
}

impl<F, G> DelegateComicSaver<F, G>
where
    F: Fn(&ComicDownloadContext) -> bool + Send + Sync,
    G: for<'a> Fn(
            &'a ComicDownloadContext,
        ) -> Pin<Box<dyn Future<Output = anf_core::Result<()>> + Send + 'a>>
        + Send
        + Sync,
{
    pub fn new(need_to_save_fn: F, save_fn: G) -> Self {
        Self {
            need_to_save_fn,
            save_fn,
        }
    }
}

#[async_trait]
impl<F, G> ComicSaver for DelegateComicSaver<F, G>
where
    F: Fn(&ComicDownloadContext) -> bool + Send + Sync,
    G: for<'a> Fn(
            &'a ComicDownloadContext,
        ) -> Pin<Box<dyn Future<Output = anf_core::Result<()>> + Send + 'a>>
        + Send
        + Sync,
{
    fn need_to_save(&self, ctx: &ComicDownloadContext) -> bool {
        (self.need_to_save_fn)(ctx)
    }

    async fn save(&self, ctx: &ComicDownloadContext) -> anf_core::Result<()> {
        (self.save_fn)(ctx).await
    }
}
