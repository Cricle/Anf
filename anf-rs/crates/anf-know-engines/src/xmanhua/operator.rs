use crate::mangabz::operator::MangabzProvider;
use anf_core::NetworkAdapter;
use std::sync::Arc;

pub struct XmanhuaProvider;
impl XmanhuaProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> MangabzProvider {
        MangabzProvider::with_base_url(network, "http://www.xmanhua.com")
    }
}
