use super::operator::Dm5Provider;
use anf_core::{ComicSourceCondition, ComicSourceContext, ComicSourceProvider, NetworkAdapter};
use std::sync::Arc;

pub struct Dm5Condition;

impl ComicSourceCondition for Dm5Condition {
    fn engine_name(&self) -> &str {
        "Dm5"
    }
    fn address(&self) -> &str {
        "http://www.dm5.com"
    }
    fn favicon_address(&self) -> &str {
        "http://www.dm5.com/favicon.ico"
    }
    fn condition(&self, ctx: &ComicSourceContext) -> bool {
        ctx.host.as_deref() == Some("www.dm5.com")
    }
    fn create_provider(&self, network: Arc<dyn NetworkAdapter>) -> Arc<dyn ComicSourceProvider> {
        Arc::new(Dm5Provider::new(network))
    }
}
