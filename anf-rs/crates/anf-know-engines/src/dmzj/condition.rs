use super::operator::DmzjProvider;
use anf_core::{ComicSourceCondition, ComicSourceContext, ComicSourceProvider, NetworkAdapter};
use std::sync::Arc;

pub struct DmzjCondition;

impl ComicSourceCondition for DmzjCondition {
    fn engine_name(&self) -> &str {
        "DMZJ"
    }
    fn address(&self) -> &str {
        "https://www.dmzj.com"
    }
    fn favicon_address(&self) -> &str {
        "https://www.dmzj.com/favicon.ico"
    }
    fn condition(&self, ctx: &ComicSourceContext) -> bool {
        matches!(
            ctx.host.as_deref(),
            Some("www.dmzj.com") | Some("manhua.dmzj.com")
        )
    }
    fn create_provider(&self, network: Arc<dyn NetworkAdapter>) -> Arc<dyn ComicSourceProvider> {
        Arc::new(DmzjProvider::new(network))
    }
}
