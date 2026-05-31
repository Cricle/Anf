use axum::{routing::get, Router};
use std::path::PathBuf;
use std::sync::Arc;
use tower_http::cors::CorsLayer;
use tower_http::services::ServeDir;

use crate::routes::reading;
use crate::state::AppState;
use anf_core::{ComicEngine, NetworkAdapter, ProposalEngine, ReqwestAdapter, SearchEngine};
use anf_know_engines::register_all_engines;
use anf_plugins::PluginLoader;

pub async fn create_app() -> anyhow::Result<Router> {
    let network: Arc<dyn NetworkAdapter> = Arc::new(ReqwestAdapter::with_default());
    let mut comic_engine = ComicEngine::new(network.clone());
    let mut search_engine = SearchEngine::new();
    let mut proposal_engine = ProposalEngine::new();

    // Register built-in Rust engines
    register_all_engines(
        &mut comic_engine,
        &mut search_engine,
        &mut proposal_engine,
        network.clone(),
    );

    // Register Lua plugins from ./plugins/ directory
    let plugin_dir = std::env::var("ANF_PLUGINS_DIR")
        .map(PathBuf::from)
        .unwrap_or_else(|_| PathBuf::from("plugins"));
    let loader = PluginLoader::new(&plugin_dir);
    let lua_count = loader.load_all(&mut comic_engine, &mut search_engine, &mut proposal_engine, network.clone());
    tracing::info!(count = lua_count, "loaded lua plugins");

    let comic_engine = Arc::new(comic_engine);
    let state = Arc::new(AppState::new(comic_engine, search_engine, proposal_engine));

    let api = Router::new()
        .route("/get-providers", get(reading::get_providers))
        .route("/search", get(reading::search))
        .route("/get-entity", get(reading::get_entity))
        .route("/get-chapter", get(reading::get_chapter))
        .route("/get-image", get(reading::get_image))
        .route("/get-proposal", get(reading::get_proposal));

    // Serve frontend static files
    let frontend_dir = std::env::var("ANF_FRONTEND_DIR")
        .map(PathBuf::from)
        .unwrap_or_else(|_| PathBuf::from("apps/frontend/dist"));
    let frontend_service = ServeDir::new(&frontend_dir);

    let app = Router::new()
        .nest("/api/v1/reading", api)
        .fallback_service(frontend_service)
        .layer(CorsLayer::permissive())
        .with_state(state);

    Ok(app)
}
