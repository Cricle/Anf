pub mod bilibili;
pub mod dm5;
pub mod jisu;
pub mod js_eval;
pub mod kuaikan;
pub mod mangabz;
pub mod tencent;
pub mod xmanhua;

use anf_core::ComicEngine;
use std::sync::Arc;

/// Register all known comic source conditions (URL matching for reader).
/// Search and proposal providers are loaded from Lua plugins.
pub fn register_all_engines(comic_engine: &mut ComicEngine) {
    comic_engine.add(Arc::new(dm5::condition::Dm5Condition));
    comic_engine.add(Arc::new(jisu::condition::JisuCondition));
    comic_engine.add(Arc::new(kuaikan::condition::KuaikanCondition));
    comic_engine.add(Arc::new(tencent::condition::TencentCondition));
    comic_engine.add(Arc::new(bilibili::condition::BilibiliCondition));
    comic_engine.add(Arc::new(mangabz::condition::MangabzCondition));
    comic_engine.add(Arc::new(xmanhua::condition::XmanhuaCondition));
}
