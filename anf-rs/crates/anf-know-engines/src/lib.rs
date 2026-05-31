pub mod bikabika;
pub mod bilibili;
pub mod dm5;
pub mod dmzj;
pub mod jisu;
pub mod js_eval;
pub mod kuaikan;
pub mod mangabz;
pub mod qimiao;
pub mod soman;
pub mod tencent;
pub mod xmanhua;

use anf_core::{ComicEngine, NetworkAdapter, ProposalDescription, ProposalEngine, SearchEngine};
use std::sync::Arc;

/// Register all known engines into the comic engine, search engine, and proposal engine
pub fn register_all_engines(
    comic_engine: &mut ComicEngine,
    search_engine: &mut SearchEngine,
    proposal_engine: &mut ProposalEngine,
    network: Arc<dyn NetworkAdapter>,
) {
    // ── Comic source conditions ────────────────────────────
    comic_engine.add(Arc::new(dm5::condition::Dm5Condition));
    comic_engine.add(Arc::new(dmzj::condition::DmzjCondition));
    comic_engine.add(Arc::new(jisu::condition::JisuCondition));
    comic_engine.add(Arc::new(kuaikan::condition::KuaikanCondition));
    comic_engine.add(Arc::new(tencent::condition::TencentCondition));
    comic_engine.add(Arc::new(bilibili::condition::BilibiliCondition));
    comic_engine.add(Arc::new(qimiao::condition::QimiaoCondition));
    comic_engine.add(Arc::new(mangabz::condition::MangabzCondition));
    comic_engine.add(Arc::new(xmanhua::condition::XmanhuaCondition));
    comic_engine.add(Arc::new(bikabika::condition::BikabikaCondition));

    // ── Search providers ───────────────────────────────────
    search_engine.add(Arc::new(soman::search::SomanSearchProvider::new(
        network.clone(),
    )));
    search_engine.add(Arc::new(dm5::search::Dm5SearchProvider::new(
        network.clone(),
    )));
    search_engine.add(Arc::new(bilibili::search::BilibiliSearchProvider::new(
        network.clone(),
    )));
    search_engine.add(Arc::new(kuaikan::search::KuaikanSearchProvider::new(
        network.clone(),
    )));
    search_engine.add(Arc::new(tencent::search::TencentSearchProvider::new(
        network.clone(),
    )));
    search_engine.add(Arc::new(mangabz::search::MangabzSearchProvider::new(
        network.clone(),
    )));
    search_engine.add(Arc::new(qimiao::search::QimiaoSearchProvider::new(
        network.clone(),
    )));
    search_engine.add(Arc::new(bikabika::search::BikabikaSearchProvider::new(
        network.clone(),
    )));
    search_engine.add(Arc::new(dmzj::search::DmzjSearchProvider::new(
        network.clone(),
    )));
    search_engine.add(Arc::new(jisu::search::JisuSearchProvider::new(
        network.clone(),
    )));
    search_engine.add(Arc::new(xmanhua::search::XmanhuaSearchProvider::new(
        network.clone(),
    )));

    // ── Proposal providers ─────────────────────────────────
    proposal_engine.add(
        ProposalDescription {
            name: "Dm5".into(),
            provider_name: "Dm5".into(),
        },
        Arc::new(dm5::proposal::Dm5ProposalProvider::new(network.clone())),
    );
    proposal_engine.add(
        ProposalDescription {
            name: "Bilibili".into(),
            provider_name: "Bilibili".into(),
        },
        Arc::new(bilibili::proposal::BilibiliProposalProvider::new(
            network.clone(),
        )),
    );
    proposal_engine.add(
        ProposalDescription {
            name: "Kuaikan".into(),
            provider_name: "Kuaikan".into(),
        },
        Arc::new(kuaikan::proposal::KuaikanProposalProvider::new(
            network.clone(),
        )),
    );
    proposal_engine.add(
        ProposalDescription {
            name: "Tencent".into(),
            provider_name: "Tencent".into(),
        },
        Arc::new(tencent::proposal::TencentProposalProvider::new(
            network.clone(),
        )),
    );
    proposal_engine.add(
        ProposalDescription {
            name: "Mangabz".into(),
            provider_name: "Mangabz".into(),
        },
        Arc::new(mangabz::proposal::MangabzProposalProvider::new(
            network.clone(),
        )),
    );
    proposal_engine.add(
        ProposalDescription {
            name: "DMZJ".into(),
            provider_name: "DMZJ".into(),
        },
        Arc::new(dmzj::proposal::DmzjProposalProvider::new(
            network.clone(),
        )),
    );
    proposal_engine.add(
        ProposalDescription {
            name: "Jisu".into(),
            provider_name: "Jisu".into(),
        },
        Arc::new(jisu::proposal::JisuProposalProvider::new(
            network.clone(),
        )),
    );
    proposal_engine.add(
        ProposalDescription {
            name: "Qimiao".into(),
            provider_name: "Qimiao".into(),
        },
        Arc::new(qimiao::proposal::QimiaoProposalProvider::new(
            network.clone(),
        )),
    );
    proposal_engine.add(
        ProposalDescription {
            name: "Bikabika".into(),
            provider_name: "Bikabika".into(),
        },
        Arc::new(bikabika::proposal::BikabikaProposalProvider::new(
            network.clone(),
        )),
    );
    proposal_engine.add(
        ProposalDescription {
            name: "Xmanhua".into(),
            provider_name: "Xmanhua".into(),
        },
        Arc::new(xmanhua::proposal::XmanhuaProposalProvider::new(
            network.clone(),
        )),
    );
}
