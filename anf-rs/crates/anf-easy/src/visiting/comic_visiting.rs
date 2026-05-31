use std::sync::Arc;
use tokio::sync::Mutex;

use crate::visiting::chapter_manager::ComicChapterManager;
use crate::visiting::interceptor::ComicVisitingInterceptor;
use crate::visiting::resource_factory::{ResourceFactory, ResourceFactoryCreator};
use anf_core::{ChapterWithPage, ComicEntity, ComicSourceProvider};

/// ComicVisiting<T>: manages lazy loading of comic chapters
/// Matches C# ComicVisiting<TResource>
pub struct ComicVisiting<T: Clone + Send + Sync + 'static> {
    address: Option<String>,
    entity: Option<ComicEntity>,
    provider: Option<Arc<dyn ComicSourceProvider>>,
    resource_factory: Option<Arc<dyn ResourceFactory<T>>>,
    chapter_with_pages: Vec<Option<ChapterWithPage>>,
    pub resource_factory_creator: Option<Box<dyn ResourceFactoryCreator<T>>>,
    pub interceptor: Option<Arc<dyn ComicVisitingInterceptor<T>>>,
    loading_mutex: Mutex<()>,
}

impl<T: Clone + Send + Sync + 'static> ComicVisiting<T> {
    pub fn new() -> Self {
        Self {
            address: None,
            entity: None,
            provider: None,
            resource_factory: None,
            chapter_with_pages: Vec::new(),
            resource_factory_creator: None,
            interceptor: None,
            loading_mutex: Mutex::new(()),
        }
    }

    pub fn address(&self) -> Option<&str> {
        self.address.as_deref()
    }

    pub fn entity(&self) -> Option<&ComicEntity> {
        self.entity.as_ref()
    }

    pub fn provider(&self) -> Option<&Arc<dyn ComicSourceProvider>> {
        self.provider.as_ref()
    }

    pub fn resource_factory(&self) -> Option<&Arc<dyn ResourceFactory<T>>> {
        self.resource_factory.as_ref()
    }

    pub fn chapter_with_pages(&self) -> &[Option<ChapterWithPage>] {
        &self.chapter_with_pages
    }

    pub fn chapter_count(&self) -> usize {
        self.chapter_with_pages.len()
    }

    pub fn erase_chapter(&mut self, index: usize) {
        if index < self.chapter_with_pages.len() {
            self.chapter_with_pages[index] = None;
        }
    }

    /// Load comic entity from provider
    pub async fn load(
        &mut self,
        address: &str,
        provider: Arc<dyn ComicSourceProvider>,
    ) -> anf_core::Result<bool> {
        self.address = Some(address.to_string());
        self.provider = Some(provider.clone());

        let entity = provider.get_chapters(address).await?;
        self.chapter_with_pages = vec![None; entity.chapters.len()];
        self.entity = Some(entity);

        if let Some(ref creator) = self.resource_factory_creator {
            let factory = creator
                .create(address, provider.clone(), self.entity.as_ref().unwrap())
                .await?;
            self.resource_factory = Some(Arc::from(factory));
        }

        Ok(true)
    }

    /// Load a specific chapter's pages
    pub async fn load_chapter(&mut self, index: usize) -> anf_core::Result<()> {
        let entity = self
            .entity
            .as_ref()
            .ok_or_else(|| anf_core::AnfError::Other("entity is null, call load() first".into()))?;

        if index >= self.chapter_with_pages.len() {
            return Err(anf_core::AnfError::Other(format!(
                "chapter index {index} out of range"
            )));
        }
        if self.chapter_with_pages[index].is_some() {
            return Ok(());
        }

        let _guard = self.loading_mutex.lock().await;

        // Double-check after acquiring lock
        if self.chapter_with_pages[index].is_some() {
            return Ok(());
        }

        let chapter = entity.chapters[index].clone();

        if let Some(ref interceptor) = self.interceptor {
            interceptor.loading_chapter(index, &chapter, entity).await?;
        }

        let provider = self
            .provider
            .as_ref()
            .ok_or_else(|| anf_core::AnfError::Other("provider not set".into()))?;
        let pages = provider.get_pages(&chapter.target_url).await?;
        let cwp = ChapterWithPage {
            chapter: chapter.clone(),
            pages,
        };

        if let Some(ref interceptor) = self.interceptor {
            interceptor.loaded_chapter(index, &cwp).await?;
        }

        self.chapter_with_pages[index] = Some(cwp);
        Ok(())
    }

    /// Get a chapter manager for the given index (loads chapter if needed)
    pub async fn get_chapter_manager(
        &mut self,
        index: usize,
    ) -> anf_core::Result<ComicChapterManager<T>> {
        self.load_chapter(index).await?;
        let cwp = self.chapter_with_pages[index]
            .clone()
            .ok_or_else(|| anf_core::AnfError::Other("chapter not loaded".into()))?;
        let provider = self
            .provider
            .clone()
            .ok_or_else(|| anf_core::AnfError::Other("provider not set".into()))?;
        Ok(ComicChapterManager::new(
            cwp,
            provider,
            self.resource_factory.clone(),
        ))
    }
}

impl<T: Clone + Send + Sync + 'static> Default for ComicVisiting<T> {
    fn default() -> Self {
        Self::new()
    }
}
