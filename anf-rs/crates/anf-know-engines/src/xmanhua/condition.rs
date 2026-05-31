use crate::mangabz::operator::MangabzProvider;
use anf_core::{ComicSourceCondition, ComicSourceContext, ComicSourceProvider, NetworkAdapter};
use std::sync::Arc;

pub struct XmanhuaCondition;
impl ComicSourceCondition for XmanhuaCondition {
    fn engine_name(&self) -> &str {
        "Xmanhua"
    }
    fn address(&self) -> &str {
        "http://www.xmanhua.com"
    }
    fn favicon_address(&self) -> &str {
        "http://www.xmanhua.com/favicon.ico"
    }
    fn condition(&self, ctx: &ComicSourceContext) -> bool {
        ctx.host.as_deref() == Some("www.xmanhua.com")
    }
    fn create_provider(&self, network: Arc<dyn NetworkAdapter>) -> Arc<dyn ComicSourceProvider> {
        Arc::new(MangabzProvider::with_base_url(
            network,
            "http://www.xmanhua.com",
        ))
    }
}
