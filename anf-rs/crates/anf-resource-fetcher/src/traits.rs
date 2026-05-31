use anf_channel_model::mongo::{AnfComicEntityTruck, WithPageChapter};
use async_trait::async_trait;

/// SingleResourceFetcher: fetches single comic resources
#[async_trait]
pub trait SingleResourceFetcher: Send + Sync {
    async fn fetch_entity(&self, url: &str) -> anf_core::Result<Option<AnfComicEntityTruck>>;
    async fn fetch_chapter(
        &self,
        url: &str,
        entity_url: &str,
    ) -> anf_core::Result<Option<WithPageChapter>>;
}

/// BatchResourceFetcher: fetches multiple comic resources
#[async_trait]
pub trait BatchResourceFetcher: Send + Sync {
    async fn fetch_entities(&self, urls: &[&str]) -> anf_core::Result<Vec<AnfComicEntityTruck>>;
    async fn fetch_chapters(&self, urls: &[(&str, &str)])
        -> anf_core::Result<Vec<WithPageChapter>>;
}

/// RootFetcher: combines single and batch fetching
#[async_trait]
pub trait RootFetcher: SingleResourceFetcher + BatchResourceFetcher {}
