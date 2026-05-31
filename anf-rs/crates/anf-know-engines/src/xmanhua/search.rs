use crate::mangabz::search::MangabzSearchProvider;
use anf_core::NetworkAdapter;
use std::sync::Arc;

/// XmanhuaSearchProvider: reuses MangabzSearchProvider with a different base URL
pub struct XmanhuaSearchProvider;
impl XmanhuaSearchProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> MangabzSearchProvider {
        MangabzSearchProvider::with_base_url(network, "http://www.xmanhua.com")
    }
}
