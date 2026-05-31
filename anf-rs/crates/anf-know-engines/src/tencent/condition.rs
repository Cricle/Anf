use super::operator::TencentProvider;
use anf_core::{ComicSourceCondition, ComicSourceContext, ComicSourceProvider, NetworkAdapter};
use std::sync::Arc;

pub struct TencentCondition;
impl ComicSourceCondition for TencentCondition {
    fn engine_name(&self) -> &str {
        "Tencent"
    }
    fn address(&self) -> &str {
        "https://ac.qq.com"
    }
    fn favicon_address(&self) -> &str {
        "https://ac.qq.com/favicon.ico"
    }
    fn condition(&self, ctx: &ComicSourceContext) -> bool {
        ctx.host.as_deref() == Some("ac.qq.com")
    }
    fn create_provider(&self, network: Arc<dyn NetworkAdapter>) -> Arc<dyn ComicSourceProvider> {
        Arc::new(TencentProvider::new(network))
    }
}
