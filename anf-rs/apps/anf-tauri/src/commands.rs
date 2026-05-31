use crate::state::{AppState, BookshelfItem};
use anf_resource_fetcher::SingleResourceFetcher;
use tauri::State;

#[tauri::command]
pub async fn get_providers(state: State<'_, AppState>) -> Result<Vec<String>, String> {
    Ok(state
        .comic_engine
        .engine_names()
        .iter()
        .map(|s| s.to_string())
        .collect())
}

#[tauri::command]
pub async fn search(
    state: State<'_, AppState>,
    keyword: String,
    skip: i32,
    take: i32,
) -> Result<anf_core::SearchComicResult, String> {
    let engine = state.search_engine.lock().await;
    engine
        .search(&keyword, skip, take)
        .await
        .map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn get_entity(
    state: State<'_, AppState>,
    url: String,
) -> Result<anf_channel_model::mongo::AnfComicEntityTruck, String> {
    state
        .remote_fetcher
        .fetch_entity(&url)
        .await
        .map_err(|e| e.to_string())?
        .ok_or_else(|| "provider not found".to_string())
}

#[tauri::command]
pub async fn get_chapter(
    state: State<'_, AppState>,
    entity_url: String,
    chapter_url: String,
) -> Result<anf_channel_model::mongo::WithPageChapter, String> {
    state
        .remote_fetcher
        .fetch_chapter(&chapter_url, &entity_url)
        .await
        .map_err(|e| e.to_string())?
        .ok_or_else(|| "chapter not found".to_string())
}

#[tauri::command]
pub async fn get_image(
    state: State<'_, AppState>,
    entity_url: String,
    url: String,
) -> Result<Vec<u8>, String> {
    let provider = state
        .comic_engine
        .create_provider(&entity_url)
        .ok_or("provider not found")?;
    let data = provider
        .get_image_stream(&url)
        .await
        .map_err(|e| e.to_string())?;
    Ok(data.to_vec())
}

#[tauri::command]
pub async fn get_proposal(
    state: State<'_, AppState>,
    engine_name: Option<String>,
    take: i32,
) -> Result<Vec<anf_core::ComicSnapshot>, String> {
    let engine = state.proposal_engine.lock().await;
    let index = if let Some(ref name) = engine_name {
        engine
            .descriptions()
            .iter()
            .position(|d| d.provider_name == *name)
            .unwrap_or(0)
    } else {
        0
    };
    engine
        .get_proposal(index, take)
        .await
        .map_err(|e| e.to_string())
}

#[tauri::command]
pub async fn get_bookshelf(state: State<'_, AppState>) -> Result<Vec<BookshelfItem>, String> {
    Ok(state.get_bookshelf())
}

#[tauri::command]
pub async fn add_to_bookshelf(
    state: State<'_, AppState>,
    url: String,
    name: String,
    image_url: String,
    descript: String,
    chapters_count: usize,
) -> Result<(), String> {
    state.add_to_bookshelf(&BookshelfItem {
        url,
        name,
        image_url,
        descript,
        current_chapter: 0,
        current_page: 0,
        chapters_count,
    });
    Ok(())
}

#[tauri::command]
pub async fn remove_from_bookshelf(state: State<'_, AppState>, url: String) -> Result<(), String> {
    state.remove_from_bookshelf(&url);
    Ok(())
}

#[tauri::command]
pub async fn update_reading_progress(
    state: State<'_, AppState>,
    url: String,
    chapter: usize,
    page: usize,
) -> Result<(), String> {
    state.update_progress(&url, chapter, page);
    Ok(())
}
