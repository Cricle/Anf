use crate::dm5::proposal::Dm5ProposalProvider;
use anf_core::NetworkAdapter;
use std::sync::Arc;

/// JisuProposalProvider: reuses Dm5ProposalProvider with a different base URL
pub struct JisuProposalProvider;
impl JisuProposalProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Dm5ProposalProvider {
        Dm5ProposalProvider::with_base_url(network, "http://www.1kkk.com")
    }
}
