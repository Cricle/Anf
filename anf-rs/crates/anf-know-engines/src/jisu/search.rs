use crate::dm5::search::Dm5SearchProvider;
use anf_core::NetworkAdapter;
use std::sync::Arc;

/// JisuSearchProvider: reuses Dm5SearchProvider with a different base URL
pub struct JisuSearchProvider;
impl JisuSearchProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Dm5SearchProvider {
        Dm5SearchProvider::with_base_url(network, "http://www.1kkk.com")
    }
}
