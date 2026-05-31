use super::operator::BilibiliProvider;
use anf_core::{ComicSourceCondition, ComicSourceContext, ComicSourceProvider, NetworkAdapter};
use std::sync::Arc;

pub struct BilibiliCondition;

impl ComicSourceCondition for BilibiliCondition {
    fn engine_name(&self) -> &str {
        "Bilibili"
    }
    fn address(&self) -> &str {
        "http://manga.bilibili.com"
    }
    fn favicon_address(&self) -> &str {
        "https://www.bilibili.com/favicon.ico"
    }
    fn condition(&self, ctx: &ComicSourceContext) -> bool {
        ctx.host.as_deref() == Some("manga.bilibili.com")
    }
    fn create_provider(&self, network: Arc<dyn NetworkAdapter>) -> Arc<dyn ComicSourceProvider> {
        Arc::new(BilibiliProvider::new(network))
    }
}
