use anf_core::{ChapterWithPage, ComicChapter, ComicEntity, ComicPage};
use async_trait::async_trait;

/// VisitingInterceptor trait
/// Matches C# IComicVisitingInterceptor<TResource>
#[async_trait]
pub trait ComicVisitingInterceptor<T>: Send + Sync {
    async fn loading_chapter(
        &self,
        _index: usize,
        _chapter: &ComicChapter,
        _entity: &ComicEntity,
    ) -> anf_core::Result<()> {
        Ok(())
    }
    async fn loaded_chapter(&self, _index: usize, _cwp: &ChapterWithPage) -> anf_core::Result<()> {
        Ok(())
    }
    async fn getting_page(&self, _index: usize, _page: &ComicPage) -> anf_core::Result<()> {
        Ok(())
    }
    async fn got_page(&self, _index: usize, _page: &ComicPage) -> anf_core::Result<()> {
        Ok(())
    }
}
