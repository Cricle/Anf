use async_trait::async_trait;
use std::sync::Arc;

use crate::models::*;
use crate::Result;

// ── ProposalProvider ───────────────────────────────────────────

#[async_trait]
pub trait ProposalProvider: Send + Sync {
    fn engine_name(&self) -> &str;
    async fn get_proposal(&self, take: i32) -> Result<Vec<ComicSnapshot>>;
}

// ── ProposalDescription ────────────────────────────────────────

#[derive(Debug, Clone)]
pub struct ProposalDescription {
    pub name: String,
    pub provider_name: String,
}

// ── ProposalEngine ─────────────────────────────────────────────

pub struct ProposalEngine {
    entries: Vec<(ProposalDescription, Arc<dyn ProposalProvider>)>,
}

impl ProposalEngine {
    pub fn new() -> Self {
        Self {
            entries: Vec::new(),
        }
    }

    pub fn add(&mut self, desc: ProposalDescription, provider: Arc<dyn ProposalProvider>) {
        self.entries.push((desc, provider));
    }

    pub fn descriptions(&self) -> Vec<&ProposalDescription> {
        self.entries.iter().map(|(d, _)| d).collect()
    }

    pub fn get_by_index(&self, index: usize) -> Option<&Arc<dyn ProposalProvider>> {
        self.entries.get(index).map(|(_, p)| p)
    }

    pub async fn get_proposal(&self, index: usize, take: i32) -> Result<Vec<ComicSnapshot>> {
        let provider = self
            .get_by_index(index)
            .ok_or_else(|| crate::AnfError::Other("proposal engine index out of range".into()))?;
        provider.get_proposal(take).await
    }
}

impl Default for ProposalEngine {
    fn default() -> Self {
        Self::new()
    }
}
