pub mod bilibili;
pub mod dm5;
pub mod jisu;
pub mod js_eval;
pub mod kuaikan;
pub mod mangabz;
pub mod tencent;
pub mod xmanhua;

use anf_core::{ComicEngine, NetworkAdapter, ProposalEngine, SearchEngine};
use std::sync::Arc;

/// Register all known engines into the comic engine.
/// Search and proposal providers are loaded from Lua plugins.
pub fn register_all_engines(
    comic_engine: &mut ComicEngine,
    _search_engine: &mut SearchEngine,
    _proposal_engine: &mut ProposalEngine,
    _network: Arc<dyn NetworkAdapter>,
) {
    // ── Comic source conditions (URL matching for reader) ──
    comic_engine.add(Arc::new(dm5::condition::Dm5Condition));
    comic_engine.add(Arc::new(jisu::condition::JisuCondition));
    comic_engine.add(Arc::new(kuaikan::condition::KuaikanCondition));
    comic_engine.add(Arc::new(tencent::condition::TencentCondition));
    comic_engine.add(Arc::new(bilibili::condition::BilibiliCondition));
    comic_engine.add(Arc::new(mangabz::condition::MangabzCondition));
    comic_engine.add(Arc::new(xmanhua::condition::XmanhuaCondition));

    // Search and proposal providers are loaded from Lua plugins (plugins/*.lua)
}
