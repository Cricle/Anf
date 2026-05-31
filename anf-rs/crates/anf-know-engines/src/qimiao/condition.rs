use super::operator::QimiaoProvider;
use anf_core::{ComicSourceCondition, ComicSourceContext, ComicSourceProvider, NetworkAdapter};
use std::sync::Arc;

pub struct QimiaoCondition;
impl ComicSourceCondition for QimiaoCondition {
    fn engine_name(&self) -> &str {
        "Qimiao"
    }
    fn address(&self) -> &str {
        "https://www.qimiaomh.com"
    }
    fn favicon_address(&self) -> &str {
        "https://www.qimiaomh.com/favicon.ico"
    }
    fn condition(&self, ctx: &ComicSourceContext) -> bool {
        ctx.host.as_deref() == Some("www.qimiaomh.com")
    }
    fn create_provider(&self, network: Arc<dyn NetworkAdapter>) -> Arc<dyn ComicSourceProvider> {
        Arc::new(QimiaoProvider::new(network))
    }
}
