use async_trait::async_trait;

use anf_core::{ComicChapter, ComicEntity, ComicPage};

// ── DownloadListenerContext ────────────────────────────────────

#[derive(Debug, Clone)]
pub struct DownloadListenerContext {
    pub entity: ComicEntity,
    pub chapter: ComicChapter,
    pub page: ComicPage,
}

// ── DownloadExceptionListenerContext ───────────────────────────

#[derive(Debug, Clone)]
pub struct DownloadExceptionListenerContext {
    pub entity: ComicEntity,
    pub chapter: ComicChapter,
    pub page: ComicPage,
    pub error: String,
}

// ── DownloadSaveListenerContext ────────────────────────────────

#[derive(Debug, Clone)]
pub struct DownloadSaveListenerContext {
    pub entity: ComicEntity,
    pub chapter: ComicChapter,
    pub page: ComicPage,
    pub data_len: usize,
}

// ── DownloadListener trait ─────────────────────────────────────

#[async_trait]
pub trait DownloadListener: Send + Sync {
    async fn ready_fetch(&self, _ctx: &DownloadListenerContext) -> anf_core::Result<()> {
        Ok(())
    }
    async fn not_need_to_save(&self, _ctx: &DownloadListenerContext) -> anf_core::Result<()> {
        Ok(())
    }
    async fn canceled(&self, _ctx: &DownloadListenerContext) -> anf_core::Result<()> {
        Ok(())
    }
    async fn begin_fetch_page(&self, _ctx: &DownloadListenerContext) -> anf_core::Result<()> {
        Ok(())
    }
    async fn fetch_page_exception(
        &self,
        _ctx: &DownloadExceptionListenerContext,
    ) -> anf_core::Result<()> {
        Ok(())
    }
    async fn end_fetch_page(&self, _ctx: &DownloadListenerContext) -> anf_core::Result<()> {
        Ok(())
    }
    async fn ready_save(&self, _ctx: &DownloadSaveListenerContext) -> anf_core::Result<()> {
        Ok(())
    }
    async fn completed_save(&self, _ctx: &DownloadListenerContext) -> anf_core::Result<()> {
        Ok(())
    }
}
