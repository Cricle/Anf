use async_trait::async_trait;
use bytes::Bytes;
use std::sync::Arc;

use crate::models::*;
use crate::network::NetworkAdapter;
use crate::Result;

// ── ComicSourceProvider ────────────────────────────────────────

#[async_trait]
pub trait ComicSourceProvider: Send + Sync {
    async fn get_chapters(&self, target_url: &str) -> Result<ComicEntity>;
    async fn get_pages(&self, target_url: &str) -> Result<Vec<ComicPage>>;
    async fn get_image_stream(&self, target_url: &str) -> Result<Bytes>;
}

// ── ComicSourceCondition ───────────────────────────────────────

pub trait ComicSourceCondition: Send + Sync {
    fn engine_name(&self) -> &str;
    fn order(&self) -> i32 {
        0
    }
    fn address(&self) -> &str;
    fn favicon_address(&self) -> &str;
    fn condition(&self, ctx: &ComicSourceContext) -> bool;
    fn create_provider(&self, network: Arc<dyn NetworkAdapter>) -> Arc<dyn ComicSourceProvider>;
}

// ── Extension: get_chapter_with_pages ──────────────────────────

pub async fn get_chapter_with_pages(
    provider: &dyn ComicSourceProvider,
    target_url: &str,
) -> Result<ComicDetail> {
    let entity = provider.get_chapters(target_url).await?;
    let mut chapters = Vec::with_capacity(entity.chapters.len());
    for chapter in &entity.chapters {
        let pages = provider.get_pages(&chapter.target_url).await?;
        chapters.push(ChapterWithPage {
            chapter: chapter.clone(),
            pages,
        });
    }
    Ok(ComicDetail { entity, chapters })
}
