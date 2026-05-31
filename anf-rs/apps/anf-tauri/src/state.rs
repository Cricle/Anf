use anf_core::{ComicEngine, NetworkAdapter, ProposalEngine, ReqwestAdapter, SearchEngine};
use anf_plugins::PluginLoader;
use anf_resource_fetcher::fetcher::RemoteFetcher;
use rusqlite::Connection;
use serde::{Deserialize, Serialize};
use std::path::PathBuf;
use std::sync::{Arc, Mutex};
use tokio::sync::Mutex as AsyncMutex;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct BookshelfItem {
    pub url: String,
    pub name: String,
    pub image_url: String,
    pub descript: String,
    pub current_chapter: usize,
    pub current_page: usize,
    pub chapters_count: usize,
}

pub struct AppState {
    pub comic_engine: Arc<ComicEngine>,
    pub search_engine: Arc<AsyncMutex<SearchEngine>>,
    pub proposal_engine: Arc<AsyncMutex<ProposalEngine>>,
    pub remote_fetcher: RemoteFetcher,
    pub db: Mutex<Connection>,
}

impl AppState {
    pub fn new() -> Self {
        let network: Arc<dyn NetworkAdapter> = Arc::new(ReqwestAdapter::with_default());
        let mut comic_engine = ComicEngine::new(network.clone());
        let mut search_engine = SearchEngine::new();
        let mut proposal_engine = ProposalEngine::new();

        anf_know_engines::register_all_engines(
            &mut comic_engine,
            &mut search_engine,
            &mut proposal_engine,
            network.clone(),
        );

        // Load Lua plugins
        let plugin_dir = std::env::var("ANF_PLUGINS_DIR")
            .map(PathBuf::from)
            .unwrap_or_else(|_| PathBuf::from("plugins"));
        let loader = PluginLoader::new(&plugin_dir);
        let lua_count = loader.load_all(&mut comic_engine, &mut search_engine, &mut proposal_engine, network.clone());
        tracing::info!(count = lua_count, "loaded lua plugins");

        let comic_engine = Arc::new(comic_engine);
        let remote_fetcher = RemoteFetcher::new(comic_engine.clone());

        // Initialize SQLite database
        let db_path = dirs::data_local_dir()
            .unwrap_or_else(|| PathBuf::from("."))
            .join("anf");
        std::fs::create_dir_all(&db_path).ok();
        let db = Connection::open(db_path.join("bookshelf.db")).expect("failed to open db");
        db.execute_batch(
            "CREATE TABLE IF NOT EXISTS bookshelf (
                url TEXT PRIMARY KEY,
                name TEXT NOT NULL,
                image_url TEXT DEFAULT '',
                descript TEXT DEFAULT '',
                current_chapter INTEGER DEFAULT 0,
                current_page INTEGER DEFAULT 0,
                chapters_count INTEGER DEFAULT 0
            );",
        )
        .expect("failed to create table");

        Self {
            comic_engine,
            search_engine: Arc::new(AsyncMutex::new(search_engine)),
            proposal_engine: Arc::new(AsyncMutex::new(proposal_engine)),
            remote_fetcher,
            db: Mutex::new(db),
        }
    }

    pub fn get_bookshelf(&self) -> Vec<BookshelfItem> {
        let db = self.db.lock().unwrap();
        let mut stmt = db.prepare("SELECT url, name, image_url, descript, current_chapter, current_page, chapters_count FROM bookshelf").unwrap();
        stmt.query_map([], |row| {
            Ok(BookshelfItem {
                url: row.get(0)?,
                name: row.get(1)?,
                image_url: row.get(2)?,
                descript: row.get(3)?,
                current_chapter: row.get(4)?,
                current_page: row.get(5)?,
                chapters_count: row.get(6)?,
            })
        })
        .unwrap()
        .filter_map(|r| r.ok())
        .collect()
    }

    pub fn add_to_bookshelf(&self, item: &BookshelfItem) {
        let db = self.db.lock().unwrap();
        db.execute(
            "INSERT OR REPLACE INTO bookshelf (url, name, image_url, descript, current_chapter, current_page, chapters_count) VALUES (?1, ?2, ?3, ?4, ?5, ?6, ?7)",
            rusqlite::params![item.url, item.name, item.image_url, item.descript, item.current_chapter, item.current_page, item.chapters_count],
        ).ok();
    }

    pub fn remove_from_bookshelf(&self, url: &str) {
        let db = self.db.lock().unwrap();
        db.execute("DELETE FROM bookshelf WHERE url = ?1", [url])
            .ok();
    }

    pub fn update_progress(&self, url: &str, chapter: usize, page: usize) {
        let db = self.db.lock().unwrap();
        db.execute(
            "UPDATE bookshelf SET current_chapter = ?1, current_page = ?2 WHERE url = ?3",
            rusqlite::params![chapter, page, url],
        )
        .ok();
    }
}
