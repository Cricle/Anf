use crate::mangabz::proposal::MangabzProposalProvider;
use anf_core::NetworkAdapter;
use std::sync::Arc;

/// XmanhuaProposalProvider: reuses MangabzProposalProvider with a different base URL
pub struct XmanhuaProposalProvider;
impl XmanhuaProposalProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> MangabzProposalProvider {
        MangabzProposalProvider::with_base_url(network, "http://www.xmanhua.com")
    }
}
