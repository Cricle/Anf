use super::operator::KuaikanProvider;
use anf_core::{ComicSourceCondition, ComicSourceContext, ComicSourceProvider, NetworkAdapter};
use std::sync::Arc;

pub struct KuaikanCondition;
impl ComicSourceCondition for KuaikanCondition {
    fn engine_name(&self) -> &str {
        "Kuaikan"
    }
    fn address(&self) -> &str {
        "https://www.kuaikanmanhua.com"
    }
    fn favicon_address(&self) -> &str {
        "https://www.kuaikanmanhua.com/favicon.ico"
    }
    fn condition(&self, ctx: &ComicSourceContext) -> bool {
        ctx.host.as_deref() == Some("www.kuaikanmanhua.com")
    }
    fn create_provider(&self, network: Arc<dyn NetworkAdapter>) -> Arc<dyn ComicSourceProvider> {
        Arc::new(KuaikanProvider::new(network))
    }
}
