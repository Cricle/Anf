use anf_core::{ComicEngine, ProposalEngine, SearchEngine};
use anf_resource_fetcher::fetcher::RemoteFetcher;
use moka::future::Cache as MokaCache;
use std::path::PathBuf;
use std::sync::Arc;

use crate::download::DownloadManager;

pub struct AppState {
    pub comic_engine: Arc<ComicEngine>,
    pub search_engine: SearchEngine,
    pub proposal_engine: ProposalEngine,
    pub remote_fetcher: RemoteFetcher,
    pub entity_cache: MokaCache<String, String>,
    pub chapter_cache: MokaCache<String, String>,
    pub download_manager: DownloadManager,
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

        let download_dir = std::env::var("ANF_DOWNLOAD_DIR")
            .map(PathBuf::from)
            .unwrap_or_else(|_| PathBuf::from("downloads"));
        let download_manager = DownloadManager::new(comic_engine.clone(), download_dir);

        Self {
            comic_engine,
            search_engine,
            proposal_engine,
            remote_fetcher,
            entity_cache,
            chapter_cache,
            download_manager,
        }
    }
}
