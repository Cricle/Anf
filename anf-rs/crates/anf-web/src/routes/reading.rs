use axum::{
    extract::{Query, State as AxumState},
    Json,
};
use serde::Deserialize;
use std::sync::Arc;

use crate::state::AppState;
use anf_channel_model::results::EntityResult;
use anf_resource_fetcher::SingleResourceFetcher;

#[derive(Deserialize)]
pub struct SearchParams {
    pub keyword: String,
    #[serde(default)]
    pub skip: i32,
    #[serde(default = "default_take")]
    pub take: i32,
}
fn default_take() -> i32 {
    20
}

#[derive(Deserialize)]
pub struct EntityParams {
    pub url: String,
}

#[derive(Deserialize)]
pub struct ChapterParams {
    pub url: String,
    pub entity_url: String,
}

#[derive(Deserialize)]
pub struct ImageParams {
    pub entity_url: String,
    pub url: String,
}

#[derive(Deserialize)]
pub struct ProposalParams {
    pub engine: Option<String>,
    #[serde(default = "default_take")]
    pub take: i32,
}

pub async fn get_providers(
    AxumState(state): AxumState<Arc<AppState>>,
) -> Json<EntityResult<Vec<String>>> {
    let names: Vec<String> = state
        .comic_engine
        .engine_names()
        .iter()
        .map(|s| s.to_string())
        .collect();
    Json(EntityResult::new(names))
}

pub async fn search(
    AxumState(state): AxumState<Arc<AppState>>,
    Query(params): Query<SearchParams>,
) -> Json<EntityResult<anf_core::SearchComicResult>> {
    match state
        .search_engine
        .search(&params.keyword, params.skip, params.take)
        .await
    {
        Ok(result) => Json(EntityResult::new(result)),
        Err(e) => Json(EntityResult::error(1i32, e.to_string())),
    }
}

pub async fn get_entity(
    AxumState(state): AxumState<Arc<AppState>>,
    Query(params): Query<EntityParams>,
) -> Json<EntityResult<anf_channel_model::mongo::AnfComicEntityTruck>> {
    // Check cache first
    if let Some(cached) = state.entity_cache.get(&params.url).await {
        if let Ok(entity) = serde_json::from_str(&cached) {
            return Json(EntityResult::new(entity));
        }
    }
    match state.remote_fetcher.fetch_entity(&params.url).await {
        Ok(Some(entity)) => {
            // Cache the result
            if let Ok(json) = serde_json::to_string(&entity) {
                state.entity_cache.insert(params.url.clone(), json).await;
            }
            Json(EntityResult::new(entity))
        }
        Ok(None) => Json(EntityResult::error(1i32, "provider not found")),
        Err(e) => Json(EntityResult::error(1i32, e.to_string())),
    }
}

pub async fn get_chapter(
    AxumState(state): AxumState<Arc<AppState>>,
    Query(params): Query<ChapterParams>,
) -> Json<EntityResult<anf_channel_model::mongo::WithPageChapter>> {
    let cache_key = format!("{}|{}", params.entity_url, params.url);
    if let Some(cached) = state.chapter_cache.get(&cache_key).await {
        if let Ok(chapter) = serde_json::from_str(&cached) {
            return Json(EntityResult::new(chapter));
        }
    }
    match state
        .remote_fetcher
        .fetch_chapter(&params.url, &params.entity_url)
        .await
    {
        Ok(Some(chapter)) => {
            if let Ok(json) = serde_json::to_string(&chapter) {
                state.chapter_cache.insert(cache_key, json).await;
            }
            Json(EntityResult::new(chapter))
        }
        Ok(None) => Json(EntityResult::error(1i32, "chapter not found")),
        Err(e) => Json(EntityResult::error(1i32, e.to_string())),
    }
}

pub async fn get_image(
    AxumState(state): AxumState<Arc<AppState>>,
    Query(params): Query<ImageParams>,
) -> Result<axum::response::Response, (axum::http::StatusCode, String)> {
    let provider = state
        .comic_engine
        .create_provider(&params.entity_url)
        .ok_or((
            axum::http::StatusCode::NO_CONTENT,
            "provider not found".into(),
        ))?;
    match provider.get_image_stream(&params.url).await {
        Ok(data) => Ok(axum::response::Response::builder()
            .header("Content-Type", "image/png")
            .body(axum::body::Body::from(data))
            .unwrap()),
        Err(e) => Err((axum::http::StatusCode::INTERNAL_SERVER_ERROR, e.to_string())),
    }
}

pub async fn get_proposal(
    AxumState(state): AxumState<Arc<AppState>>,
    Query(params): Query<ProposalParams>,
) -> Json<EntityResult<Vec<anf_core::ComicSnapshot>>> {
    let index = if let Some(ref name) = params.engine {
        state
            .proposal_engine
            .descriptions()
            .iter()
            .position(|d| d.provider_name == *name)
            .unwrap_or(0)
    } else {
        0
    };
    match state.proposal_engine.get_proposal(index, params.take).await {
        Ok(snapshots) => Json(EntityResult::new(snapshots)),
        Err(e) => Json(EntityResult::error(1i32, e.to_string())),
    }
}
