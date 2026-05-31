use anf_core::{ComicEngine, NetworkAdapter, ProposalEngine, ReqwestAdapter, SearchEngine};
use anf_know_engines::register_all_engines;
use std::sync::Arc;

#[tokio::test]
async fn test_all_proposals() {
    let network: Arc<dyn NetworkAdapter> = Arc::new(ReqwestAdapter::with_default());
    let mut comic_engine = ComicEngine::new(network.clone());
    let mut search_engine = SearchEngine::new();
    let mut proposal_engine = ProposalEngine::new();

    register_all_engines(
        &mut comic_engine,
        &mut search_engine,
        &mut proposal_engine,
        network,
    );

    let descs = proposal_engine.descriptions().to_vec();
    let mut pass = 0;
    let mut fail = 0;

    for (i, desc) in descs.iter().enumerate() {
        match proposal_engine.get_proposal(i, 2).await {
            Ok(snapshots) => {
                if snapshots.is_empty() {
                    eprintln!("WARN {}: empty result", desc.name);
                } else {
                    eprintln!("PASS {}: {} snapshots", desc.name, snapshots.len());
                    pass += 1;
                }
            }
            Err(e) => {
                eprintln!("FAIL {}: {}", desc.name, e);
                fail += 1;
            }
        }
    }

    eprintln!("\nProposal: {} pass, {} fail", pass, fail);
}

#[tokio::test]
async fn test_all_searches() {
    let network: Arc<dyn NetworkAdapter> = Arc::new(ReqwestAdapter::with_default());
    let mut comic_engine = ComicEngine::new(network.clone());
    let mut search_engine = SearchEngine::new();
    let mut proposal_engine = ProposalEngine::new();

    register_all_engines(
        &mut comic_engine,
        &mut search_engine,
        &mut proposal_engine,
        network,
    );

    let keyword = "naruto";
    let providers = search_engine.providers().to_vec();
    let mut pass = 0;
    let mut fail = 0;

    for provider in &providers {
        match provider.search(keyword, 0, 2).await {
            Ok(result) => {
                if result.snapshots.is_empty() {
                    eprintln!("WARN {}: empty result", provider.engine_name());
                } else {
                    eprintln!("PASS {}: {} results", provider.engine_name(), result.snapshots.len());
                    pass += 1;
                }
            }
            Err(e) => {
                eprintln!("FAIL {}: {}", provider.engine_name(), e);
                fail += 1;
            }
        }
    }

    eprintln!("\nSearch: {} pass, {} fail", pass, fail);
}
