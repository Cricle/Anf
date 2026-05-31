use async_trait::async_trait;
use chrono::Utc;
use std::sync::Arc;

use crate::traits::SingleResourceFetcher;
use anf_channel_model::mongo::{AnfComicEntityInfoOnly, AnfComicEntityTruck, WithPageChapter};
use anf_core::{ComicEngine, ComicSourceProvider};

/// RemoteFetcher: fetches comic resources from remote sources
/// Matches C# RemoteFetcher
pub struct RemoteFetcher {
    engine: Arc<ComicEngine>,
}

impl RemoteFetcher {
    pub fn new(engine: Arc<ComicEngine>) -> Self {
        Self { engine }
    }

    fn get_provider(&self, url: &str) -> Option<Arc<dyn ComicSourceProvider>> {
        self.engine.create_provider(url)
    }
}

#[async_trait]
impl SingleResourceFetcher for RemoteFetcher {
    async fn fetch_entity(&self, url: &str) -> anf_core::Result<Option<AnfComicEntityTruck>> {
        let provider = match self.get_provider(url) {
            Some(p) => p,
            None => return Ok(None),
        };
        let entity = provider.get_chapters(url).await?;
        let now = Utc::now().timestamp_nanos_opt().unwrap_or_default();
        Ok(Some(AnfComicEntityTruck {
            info: AnfComicEntityInfoOnly {
                comic_url: entity.info.comic_url,
                name: entity.info.name,
                descript: entity.info.descript,
                image_url: entity.info.image_url,
                create_time: now,
                update_time: now,
            },
            chapters: entity.chapters,
        }))
    }

    async fn fetch_chapter(
        &self,
        url: &str,
        entity_url: &str,
    ) -> anf_core::Result<Option<WithPageChapter>> {
        let provider = match self.get_provider(entity_url) {
            Some(p) => p,
            None => return Ok(None),
        };
        // First get entity to find chapter title
        let entity = provider.get_chapters(entity_url).await?;
        let chapter = entity.chapters.iter().find(|c| c.target_url == url);
        let title = chapter.map(|c| c.title.clone()).unwrap_or_default();

        let pages = provider.get_pages(url).await?;
        let now = Utc::now().timestamp_nanos_opt().unwrap_or_default();
        Ok(Some(WithPageChapter {
            target_url: url.to_string(),
            title,
            pages,
            create_time: now,
            update_time: now,
            ref_count: 0,
        }))
    }
}
