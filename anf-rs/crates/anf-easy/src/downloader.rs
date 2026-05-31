use std::sync::Arc;
use tokio_util::sync::CancellationToken;

use crate::listener::{
    DownloadExceptionListenerContext, DownloadListener, DownloadListenerContext,
    DownloadSaveListenerContext,
};
use crate::saver::{ComicDownloadContext, ComicSaver};
use anf_core::{ComicChapter, ComicDetail, ComicEntity, ComicPage, ComicSourceProvider};

#[derive(Debug, Clone)]
pub struct DownloadItemRequest {
    pub chapter: ComicChapter,
    pub page: ComicPage,
}

pub struct ComicDownloadRequest {
    pub saver: Arc<dyn ComicSaver>,
    pub entity: ComicEntity,
    pub detail: Option<ComicDetail>,
    pub download_requests: Vec<DownloadItemRequest>,
    pub provider: Arc<dyn ComicSourceProvider>,
    pub listener: Option<Arc<dyn DownloadListener>>,
}

pub struct ComicDownloader;

impl ComicDownloader {
    pub fn new() -> Self {
        Self
    }

    /// Run all download tasks sequentially
    pub async fn run(
        request: &ComicDownloadRequest,
        token: CancellationToken,
    ) -> anf_core::Result<()> {
        for item in &request.download_requests {
            if token.is_cancelled() {
                break;
            }
            Self::download_page(
                request.provider.clone(),
                request.saver.clone(),
                request.listener.clone(),
                request.entity.clone(),
                item.chapter.clone(),
                item.page.clone(),
                token.clone(),
            )
            .await?;
        }
        Ok(())
    }

    async fn download_page(
        provider: Arc<dyn ComicSourceProvider>,
        saver: Arc<dyn ComicSaver>,
        listener: Option<Arc<dyn DownloadListener>>,
        entity: ComicEntity,
        chapter: ComicChapter,
        page: ComicPage,
        token: CancellationToken,
    ) -> anf_core::Result<()> {
        let listener_ctx = DownloadListenerContext {
            entity: entity.clone(),
            chapter: chapter.clone(),
            page: page.clone(),
        };
        if let Some(ref l) = listener {
            l.ready_fetch(&listener_ctx).await?;
        }
        if token.is_cancelled() {
            if let Some(ref l) = listener {
                l.canceled(&listener_ctx).await?;
            }
            return Ok(());
        }
        let check_ctx = ComicDownloadContext {
            entity: entity.clone(),
            chapter: chapter.clone(),
            page: page.clone(),
            source_data: None,
        };
        if !saver.need_to_save(&check_ctx) {
            if let Some(ref l) = listener {
                l.not_need_to_save(&listener_ctx).await?;
            }
            return Ok(());
        }
        if let Some(ref l) = listener {
            l.begin_fetch_page(&listener_ctx).await?;
        }
        match provider.get_image_stream(&page.target_url).await {
            Ok(data) => {
                if let Some(ref l) = listener {
                    l.ready_save(&DownloadSaveListenerContext {
                        entity: entity.clone(),
                        chapter: chapter.clone(),
                        page: page.clone(),
                        data_len: data.len(),
                    })
                    .await?;
                }
                let save_ctx = ComicDownloadContext {
                    entity: entity.clone(),
                    chapter: chapter.clone(),
                    page: page.clone(),
                    source_data: Some(data),
                };
                saver.save(&save_ctx).await?;
                if let Some(ref l) = listener {
                    l.end_fetch_page(&listener_ctx).await?;
                }
            }
            Err(e) => {
                if let Some(ref l) = listener {
                    l.fetch_page_exception(&DownloadExceptionListenerContext {
                        entity: entity.clone(),
                        chapter: chapter.clone(),
                        page: page.clone(),
                        error: e.to_string(),
                    })
                    .await?;
                }
            }
        }
        if let Some(ref l) = listener {
            l.completed_save(&listener_ctx).await?;
        }
        Ok(())
    }
}

impl Default for ComicDownloader {
    fn default() -> Self {
        Self::new()
    }
}
