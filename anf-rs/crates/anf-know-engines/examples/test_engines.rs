use anf_core::{ComicEngine, NetworkAdapter, ProposalEngine, ReqwestAdapter, SearchEngine};
use anf_know_engines::register_all_engines;
use anf_plugins::PluginLoader;
use std::path::PathBuf;
use std::sync::Arc;

#[tokio::main]
async fn main() {
    let network: Arc<dyn NetworkAdapter> = Arc::new(ReqwestAdapter::with_default());
    let mut comic_engine = ComicEngine::new(network.clone());
    let mut search_engine = SearchEngine::new();
    let mut proposal_engine = ProposalEngine::new();

    register_all_engines(
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
    let lua_count = loader.load_all(&mut comic_engine, &mut search_engine, &mut proposal_engine, network);
    println!("Loaded {lua_count} Lua plugin(s)\n");

    println!("=== Testing Proposal Engines ===");
    let descs = proposal_engine.descriptions().to_vec();
    for (i, desc) in descs.iter().enumerate() {
        print!("{}: ", desc.name);
        match proposal_engine.get_proposal(i, 2).await {
            Ok(snapshots) => {
                if snapshots.is_empty() {
                    println!("EMPTY");
                } else {
                    println!("PASS ({} snapshots)", snapshots.len());
                    for snap in &snapshots {
                        println!("  - {}", snap.name);
                    }
                }
            }
            Err(e) => {
                println!("FAIL: {}", e);
            }
        }
    }

    println!("\n=== Testing Search Engines ===");
    let keyword = "naruto";
    let providers = search_engine.providers().to_vec();
    for provider in &providers {
        print!("{}: ", provider.engine_name());
        match provider.search(keyword, 0, 2).await {
            Ok(result) => {
                if result.snapshots.is_empty() {
                    println!("EMPTY");
                } else {
                    println!("PASS ({} results)", result.snapshots.len());
                    for snap in result.snapshots.iter().take(2) {
                        println!("  - {}", snap.name);
                    }
                }
            }
            Err(e) => {
                println!("FAIL: {}", e);
            }
        }
    }
}
