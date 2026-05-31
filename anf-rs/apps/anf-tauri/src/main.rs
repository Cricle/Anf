#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

mod commands;
mod state;

fn main() {
    tauri::Builder::default()
        .plugin(tauri_plugin_shell::init())
        .manage(state::AppState::new())
        .invoke_handler(tauri::generate_handler![
            commands::get_providers,
            commands::search,
            commands::get_entity,
            commands::get_chapter,
            commands::get_image,
            commands::get_proposal,
            commands::get_bookshelf,
            commands::add_to_bookshelf,
            commands::remove_from_bookshelf,
            commands::update_reading_progress,
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
