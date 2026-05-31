use super::operator::MangabzProvider;
use anf_core::{ComicSourceCondition, ComicSourceContext, ComicSourceProvider, NetworkAdapter};
use std::sync::Arc;

pub struct MangabzCondition;
impl ComicSourceCondition for MangabzCondition {
    fn engine_name(&self) -> &str {
        "Mangabz"
    }
    fn address(&self) -> &str {
        "http://www.mangabz.com"
    }
    fn favicon_address(&self) -> &str {
        "http://www.mangabz.com/favicon.ico"
    }
    fn condition(&self, ctx: &ComicSourceContext) -> bool {
        ctx.host.as_deref() == Some("www.mangabz.com")
    }
    fn create_provider(&self, network: Arc<dyn NetworkAdapter>) -> Arc<dyn ComicSourceProvider> {
        Arc::new(MangabzProvider::new(network))
    }
}
