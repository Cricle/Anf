use crate::dm5::operator::Dm5Provider;
use anf_core::{ComicSourceCondition, ComicSourceContext, ComicSourceProvider, NetworkAdapter};
use std::sync::Arc;

pub struct JisuCondition;
impl ComicSourceCondition for JisuCondition {
    fn engine_name(&self) -> &str {
        "Jisu"
    }
    fn address(&self) -> &str {
        "http://www.1kkk.com"
    }
    fn favicon_address(&self) -> &str {
        "http://www.1kkk.com/favicon.ico"
    }
    fn condition(&self, ctx: &ComicSourceContext) -> bool {
        ctx.host.as_deref() == Some("www.1kkk.com")
    }
    fn create_provider(&self, network: Arc<dyn NetworkAdapter>) -> Arc<dyn ComicSourceProvider> {
        Arc::new(Dm5Provider::with_base_url(network, "http://www.1kkk.com/"))
    }
}
