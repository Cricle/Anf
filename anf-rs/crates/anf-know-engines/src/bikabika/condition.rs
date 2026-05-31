use super::operator::BikabikaProvider;
use anf_core::{ComicSourceCondition, ComicSourceContext, ComicSourceProvider, NetworkAdapter};
use std::sync::Arc;

pub struct BikabikaCondition;
impl ComicSourceCondition for BikabikaCondition {
    fn engine_name(&self) -> &str {
        "Bikabika"
    }
    fn address(&self) -> &str {
        "http://www.bikabika.com"
    }
    fn favicon_address(&self) -> &str {
        "http://www.bikabika.com/favicon.ico"
    }
    fn condition(&self, ctx: &ComicSourceContext) -> bool {
        ctx.host.as_deref() == Some("www.bikabika.com")
    }
    fn create_provider(&self, network: Arc<dyn NetworkAdapter>) -> Arc<dyn ComicSourceProvider> {
        Arc::new(BikabikaProvider::new(network))
    }
}
