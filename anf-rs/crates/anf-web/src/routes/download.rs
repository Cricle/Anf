use axum::{
    extract::{Query, State as AxumState},
    http::header,
    response::{IntoResponse, Response},
    Json,
};
use serde::Deserialize;
use std::sync::Arc;

use crate::state::AppState;
use anf_channel_model::results::EntityResult;

#[derive(Deserialize)]
pub struct DownloadStartParams {
    pub url: String,
    pub name: String,
}

#[derive(Deserialize)]
pub struct DownloadUrlParams {
    pub url: String,
}

pub async fn list_downloads(
    AxumState(state): AxumState<Arc<AppState>>,
) -> Json<EntityResult<Vec<crate::download::DownloadInfo>>> {
    let downloads = state.download_manager.get_all().await;
    Json(EntityResult::new(downloads))
}

pub async fn start_download(
    AxumState(state): AxumState<Arc<AppState>>,
    Query(params): Query<DownloadStartParams>,
) -> Json<EntityResult<String>> {
    match state
        .download_manager
        .start(params.url, params.name)
        .await
    {
        Ok(()) => Json(EntityResult::new("ok".to_string())),
        Err(e) => Json(EntityResult::error(1i32, e)),
    }
}

pub async fn download_status(
    AxumState(state): AxumState<Arc<AppState>>,
    Query(params): Query<DownloadUrlParams>,
) -> Json<EntityResult<Option<crate::download::DownloadInfo>>> {
    let info = state.download_manager.get(&params.url).await;
    Json(EntityResult::new(info))
}

pub async fn cancel_download(
    AxumState(state): AxumState<Arc<AppState>>,
    Query(params): Query<DownloadUrlParams>,
) -> Json<EntityResult<String>> {
    state.download_manager.cancel(&params.url).await;
    Json(EntityResult::new("ok".to_string()))
}

pub async fn export_pdf(
    AxumState(state): AxumState<Arc<AppState>>,
    Query(params): Query<DownloadNameParams>,
) -> Response {
    let comic_dir = state.download_manager.output_dir().join(&params.name);
    if !comic_dir.exists() {
        return Json(EntityResult::<()>::error(1i32, "download not found".to_string())).into_response();
    }

    match crate::pdf::comic_to_pdf(&comic_dir).await {
        Ok(pdf_bytes) => (
            [
                (header::CONTENT_TYPE, "application/pdf"),
                (
                    header::CONTENT_DISPOSITION,
                    &format!("attachment; filename=\"{}.pdf\"", params.name),
                ),
            ],
            pdf_bytes,
        )
            .into_response(),
        Err(e) => Json(EntityResult::<()>::error(1i32, e)).into_response(),
    }
}

#[derive(Deserialize)]
pub struct DownloadNameParams {
    pub name: String,
}
