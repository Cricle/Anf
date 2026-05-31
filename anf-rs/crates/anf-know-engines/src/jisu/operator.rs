use crate::dm5::operator::Dm5Provider;
use anf_core::NetworkAdapter;
use std::sync::Arc;

/// JisuComicOperator: reuses Dm5Provider with a different base URL
pub struct JisuProvider;
impl JisuProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Dm5Provider {
        Dm5Provider::with_base_url(network, "http://www.1kkk.com/")
    }
}
