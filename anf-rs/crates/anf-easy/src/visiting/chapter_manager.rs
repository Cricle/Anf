use crate::visiting::page_box::PageBox;
use crate::visiting::resource_factory::ResourceFactory;
use anf_core::{ChapterWithPage, ComicSourceProvider};
use std::sync::Arc;

/// ComicChapterManager: manages pages within a chapter
/// Matches C# ComicChapterManager<TResource>
pub struct ComicChapterManager<T: Clone + Send + Sync + 'static> {
    pub chapter_with_page: ChapterWithPage,
    #[allow(dead_code)]
    provider: Arc<dyn ComicSourceProvider>,
    resource_factory: Option<Arc<dyn ResourceFactory<T>>>,
}

impl<T: Clone + Send + Sync + 'static> ComicChapterManager<T> {
    pub fn new(
        chapter_with_page: ChapterWithPage,
        #[allow(dead_code)] provider: Arc<dyn ComicSourceProvider>,
        resource_factory: Option<Arc<dyn ResourceFactory<T>>>,
    ) -> Self {
        Self {
            chapter_with_page,
            provider,
            resource_factory,
        }
    }

    pub async fn get_visit_page(&self, index: usize) -> anf_core::Result<PageBox<T>> {
        let page = self
            .chapter_with_page
            .pages
            .get(index)
            .ok_or_else(|| anf_core::AnfError::Other(format!("page index {index} out of range")))?
            .clone();

        let resource = if let Some(ref factory) = self.resource_factory {
            Some(factory.get(&page.target_url).await?)
        } else {
            None
        };

        Ok(PageBox { page, resource })
    }

    pub fn page_count(&self) -> usize {
        self.chapter_with_page.pages.len()
    }
}
