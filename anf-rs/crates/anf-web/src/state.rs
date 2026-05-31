use anf_core::{ComicEngine, ProposalEngine, SearchEngine};
use anf_resource_fetcher::fetcher::RemoteFetcher;
use moka::future::Cache as MokaCache;
use std::sync::Arc;

/// AppState: shared application state for the web server with moka cache
pub struct AppState {
    pub comic_engine: Arc<ComicEngine>,
    pub search_engine: SearchEngine,
    pub proposal_engine: ProposalEngine,
    pub remote_fetcher: RemoteFetcher,
    /// Entity cache: url -> serialized AnfComicEntityTruck JSON (5 min TTL)
    pub entity_cache: MokaCache<String, String>,
    /// Chapter cache: url -> serialized WithPageChapter JSON (5 min TTL)
    pub chapter_cache: MokaCache<String, String>,
}

impl AppState {
    pub fn new(
        comic_engine: Arc<ComicEngine>,
        search_engine: SearchEngine,
        proposal_engine: ProposalEngine,
    ) -> Self {
        let remote_fetcher = RemoteFetcher::new(comic_engine.clone());
        let entity_cache = MokaCache::builder()
            .max_capacity(1000)
            .time_to_live(std::time::Duration::from_secs(300))
            .build();
        let chapter_cache = MokaCache::builder()
            .max_capacity(5000)
            .time_to_live(std::time::Duration::from_secs(300))
            .build();
        Self {
            comic_engine,
            search_engine,
            proposal_engine,
            remote_fetcher,
            entity_cache,
            chapter_cache,
        }
    }
}
