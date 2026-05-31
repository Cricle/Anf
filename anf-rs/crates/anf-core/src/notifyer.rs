use async_trait::async_trait;

use crate::models::*;

/// ChapterAnalysisNotifyer: callback for chapter analysis progress
/// Matches C# IChapterAnalysisNotifyer
#[async_trait]
pub trait ChapterAnalysisNotifyer: Send + Sync {
    async fn fetching_comic(&self, _ctx: &ComicAnalysingContext) -> crate::Result<()> {
        Ok(())
    }
    async fn fetched_comic(&self, _ctx: &ComicAnalysedContext) -> crate::Result<()> {
        Ok(())
    }
    async fn fetching_chapter(&self, _ctx: &ChapterAnalysingContext) -> crate::Result<()> {
        Ok(())
    }
    async fn fetched_chapter(&self, _ctx: &ChapterAnalysedContext) -> crate::Result<()> {
        Ok(())
    }
}
