use std::sync::Arc;

use crate::models::ComicSourceContext;
use crate::network::NetworkAdapter;
use crate::provider::{ComicSourceCondition, ComicSourceProvider};

/// ComicEngine: registry of comic source conditions, sorted by order (descending).
/// Matches C# ComicEngine : ObservableCollection<IComicSourceCondition>
pub struct ComicEngine {
    conditions: Vec<Arc<dyn ComicSourceCondition>>,
    network: Arc<dyn NetworkAdapter>,
}

impl ComicEngine {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self {
            conditions: Vec::new(),
            network,
        }
    }

    pub fn add(&mut self, condition: Arc<dyn ComicSourceCondition>) {
        self.conditions.push(condition);
        self.conditions.sort_by(|a, b| b.order().cmp(&a.order()));
    }

    pub fn remove_by_name(&mut self, name: &str) {
        self.conditions.retain(|c| c.engine_name() != name);
    }

    pub fn conditions(&self) -> &[Arc<dyn ComicSourceCondition>] {
        &self.conditions
    }

    /// Find the condition that matches the given URL
    pub fn get_provider_type(&self, target_url: &str) -> Option<&Arc<dyn ComicSourceCondition>> {
        let ctx = match ComicSourceContext::new(target_url) {
            Ok(ctx) => ctx,
            Err(_) => return None,
        };
        self.conditions.iter().find(|c| c.condition(&ctx))
    }

    /// Create a provider for the given URL
    pub fn create_provider(&self, target_url: &str) -> Option<Arc<dyn ComicSourceProvider>> {
        let condition = self.get_provider_type(target_url)?;
        Some(condition.create_provider(self.network.clone()))
    }

    /// Get engine names
    pub fn engine_names(&self) -> Vec<&str> {
        self.conditions.iter().map(|c| c.engine_name()).collect()
    }

    pub fn network(&self) -> &Arc<dyn NetworkAdapter> {
        &self.network
    }
}
