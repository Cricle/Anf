use serde::{Deserialize, Serialize};
use std::collections::HashMap;
use std::path::PathBuf;
use std::sync::Arc;
use tokio::sync::Mutex;

use anf_core::ComicEngine;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct DownloadInfo {
    pub url: String,
    pub name: String,
    pub status: DownloadStatus,
    pub total_pages: usize,
    pub downloaded_pages: usize,
    pub current_chapter: String,
    pub error: Option<String>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "snake_case")]
pub enum DownloadStatus {
    Preparing,
    Downloading,
    Completed,
    Failed,
    Cancelled,
}

pub struct DownloadManager {
    downloads: Arc<Mutex<HashMap<String, DownloadInfo>>>,
    comic_engine: Arc<ComicEngine>,
    output_dir: PathBuf,
}

impl DownloadManager {
    pub fn new(comic_engine: Arc<ComicEngine>, output_dir: PathBuf) -> Self {
        Self {
            downloads: Arc::new(Mutex::new(HashMap::new())),
            comic_engine,
            output_dir,
        }
    }

    pub fn output_dir(&self) -> &PathBuf {
        &self.output_dir
    }

    pub async fn get_all(&self) -> Vec<DownloadInfo> {
        self.downloads.lock().await.values().cloned().collect()
    }

    pub async fn get(&self, url: &str) -> Option<DownloadInfo> {
        self.downloads.lock().await.get(url).cloned()
    }

    pub async fn start(&self, url: String, name: String) -> Result<(), String> {
        let mut downloads = self.downloads.lock().await;
        if downloads.contains_key(&url) {
            return Err("already downloading".into());
        }

        downloads.insert(
            url.clone(),
            DownloadInfo {
                url: url.clone(),
                name: name.clone(),
                status: DownloadStatus::Preparing,
                total_pages: 0,
                downloaded_pages: 0,
                current_chapter: String::new(),
                error: None,
            },
        );
        drop(downloads);

        let downloads = self.downloads.clone();
        let engine = self.comic_engine.clone();
        let output_dir = self.output_dir.clone();

        tokio::spawn(async move {
            if let Err(e) = Self::run_download(&downloads, &engine, &output_dir, &url, &name).await
            {
                let mut downloads = downloads.lock().await;
                if let Some(info) = downloads.get_mut(&url) {
                    info.status = DownloadStatus::Failed;
                    info.error = Some(e);
                }
            }
        });

        Ok(())
    }

    pub async fn cancel(&self, url: &str) {
        let mut downloads = self.downloads.lock().await;
        if let Some(info) = downloads.get_mut(url) {
            info.status = DownloadStatus::Cancelled;
        }
    }

    async fn run_download(
        downloads: &Arc<Mutex<HashMap<String, DownloadInfo>>>,
        engine: &ComicEngine,
        output_dir: &PathBuf,
        url: &str,
        name: &str,
    ) -> Result<(), String> {
        let provider = engine
            .create_provider(url)
            .ok_or("no provider found for URL")?;

        let entity = provider
            .get_chapters(url)
            .await
            .map_err(|e| format!("fetch chapters: {e}"))?;

        let comic_dir = output_dir.join(sanitize_filename(name));
        tokio::fs::create_dir_all(&comic_dir)
            .await
            .map_err(|e| format!("create dir: {e}"))?;

        // Count total pages
        let mut total_pages = 0;
        for ch in &entity.chapters {
            let pages = provider
                .get_pages(&ch.target_url)
                .await
                .map_err(|e| format!("fetch pages: {e}"))?;
            total_pages += pages.len();
        }

        {
            let mut downloads = downloads.lock().await;
            if let Some(info) = downloads.get_mut(url) {
                info.total_pages = total_pages;
                info.status = DownloadStatus::Downloading;
            }
        }

        let mut downloaded = 0;
        for ch in &entity.chapters {
            {
                let mut downloads = downloads.lock().await;
                if let Some(info) = downloads.get_mut(url) {
                    if matches!(info.status, DownloadStatus::Cancelled) {
                        return Ok(());
                    }
                    info.current_chapter = ch.title.clone();
                }
            }

            let pages = provider
                .get_pages(&ch.target_url)
                .await
                .map_err(|e| format!("fetch pages: {e}"))?;

            let ch_dir = comic_dir.join(sanitize_filename(&ch.title));
            tokio::fs::create_dir_all(&ch_dir)
                .await
                .map_err(|e| format!("create chapter dir: {e}"))?;

            for (i, page) in pages.iter().enumerate() {
                {
                    let mut downloads = downloads.lock().await;
                    if let Some(info) = downloads.get_mut(url) {
                        if matches!(info.status, DownloadStatus::Cancelled) {
                            return Ok(());
                        }
                    }
                }

                let ext = guess_ext(&page.target_url);
                let filename = format!("{:04}{}", i + 1, ext);
                let filepath = ch_dir.join(&filename);

                if !filepath.exists() {
                    match provider.get_image_stream(&page.target_url).await {
                        Ok(data) => {
                            tokio::fs::write(&filepath, &data)
                                .await
                                .map_err(|e| format!("write image: {e}"))?;
                        }
                        Err(e) => {
                            tracing::warn!(error = %e, page = %page.target_url, "failed to download page");
                        }
                    }
                }

                downloaded += 1;
                let mut downloads = downloads.lock().await;
                if let Some(info) = downloads.get_mut(url) {
                    info.downloaded_pages = downloaded;
                }
            }
        }

        {
            let mut downloads = downloads.lock().await;
            if let Some(info) = downloads.get_mut(url) {
                info.status = DownloadStatus::Completed;
            }
        }

        Ok(())
    }
}

fn sanitize_filename(name: &str) -> String {
    name.chars()
        .map(|c| match c {
            '/' | '\\' | ':' | '*' | '?' | '"' | '<' | '>' | '|' => '_',
            c => c,
        })
        .collect()
}

fn guess_ext(url: &str) -> String {
    let path = url.split('?').next().unwrap_or("");
    if let Some(ext) = path.rsplit('.').next() {
        let ext = ext.to_lowercase();
        if ["jpg", "jpeg", "png", "gif", "webp", "bmp"].contains(&ext.as_str()) {
            return format!(".{ext}");
        }
    }
    ".jpg".to_string()
}
