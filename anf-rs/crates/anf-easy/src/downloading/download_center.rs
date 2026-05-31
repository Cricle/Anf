use std::collections::HashMap;
use std::sync::Arc;
use tokio::sync::Mutex;
use tokio_util::sync::CancellationToken;

use crate::downloader::{ComicDownloadRequest, ComicDownloader, DownloadItemRequest};
use crate::downloading::download_box::DownloadBox;
use crate::downloading::download_task::DownloadTask;
use crate::saver::ComicSaver;
use anf_core::{ComicDetail, ComicEntity, ComicSourceProvider};

/// DownloadCenter: manages comic download tasks
pub struct DownloadCenter {
    downloads: Mutex<HashMap<String, DownloadBox>>,
    saver: Arc<dyn ComicSaver>,
    provider: Arc<dyn ComicSourceProvider>,
}

impl DownloadCenter {
    pub fn new(saver: Arc<dyn ComicSaver>, provider: Arc<dyn ComicSourceProvider>) -> Self {
        Self {
            downloads: Mutex::new(HashMap::new()),
            saver,
            provider,
        }
    }

    pub async fn add(
        &self,
        address: &str,
        entity: ComicEntity,
        detail: ComicDetail,
    ) -> anf_core::Result<()> {
        let mut downloads = self.downloads.lock().await;
        if downloads.contains_key(address) {
            return Ok(());
        }

        let download_requests: Vec<DownloadItemRequest> = detail
            .chapters
            .iter()
            .flat_map(|cwp| {
                cwp.pages.iter().map(move |page| DownloadItemRequest {
                    chapter: cwp.chapter.clone(),
                    page: page.clone(),
                })
            })
            .collect();

        let item_count = download_requests.len();
        let token = CancellationToken::new();
        let task = DownloadTask::new(item_count, token.clone());
        let box_ = DownloadBox::new(task, address.to_string());
        downloads.insert(address.to_string(), box_);

        // Spawn download in background
        let request = ComicDownloadRequest {
            saver: self.saver.clone(),
            entity,
            detail: Some(detail),
            download_requests,
            provider: self.provider.clone(),
            listener: None,
        };
        let token_clone = token.clone();
        tokio::spawn(async move {
            if let Err(e) = ComicDownloader::run(&request, token_clone).await {
                tracing::warn!(error = %e, "download failed");
            }
        });

        Ok(())
    }

    pub async fn remove(&self, address: &str) -> bool {
        let mut downloads = self.downloads.lock().await;
        if let Some(box_) = downloads.remove(address) {
            box_.cancel();
            true
        } else {
            false
        }
    }

    pub async fn clear(&self) {
        let mut downloads = self.downloads.lock().await;
        for (_, box_) in downloads.drain() {
            box_.cancel();
        }
    }

    pub async fn contains(&self, address: &str) -> bool {
        self.downloads.lock().await.contains_key(address)
    }

    pub async fn count(&self) -> usize {
        self.downloads.lock().await.len()
    }
}
