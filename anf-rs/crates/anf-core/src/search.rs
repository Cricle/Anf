use async_trait::async_trait;
use std::sync::Arc;

use crate::models::*;
use crate::Result;

// ── SearchProvider ─────────────────────────────────────────────

#[async_trait]
pub trait SearchProvider: Send + Sync {
    fn engine_name(&self) -> &str;
    async fn search(&self, keyword: &str, skip: i32, take: i32) -> Result<SearchComicResult>;
}

// ── SearchEngine ───────────────────────────────────────────────

pub struct SearchEngine {
    providers: Vec<Arc<dyn SearchProvider>>,
}

impl SearchEngine {
    pub fn new() -> Self {
        Self {
            providers: Vec::new(),
        }
    }

    pub fn add(&mut self, provider: Arc<dyn SearchProvider>) {
        self.providers.push(provider);
    }

    pub fn providers(&self) -> &[Arc<dyn SearchProvider>] {
        &self.providers
    }

    pub fn get_provider(&self, name: &str) -> Option<&Arc<dyn SearchProvider>> {
        self.providers.iter().find(|p| p.engine_name() == name)
    }

    pub async fn search(&self, keyword: &str, skip: i32, take: i32) -> Result<SearchComicResult> {
        let mut all_snapshots = Vec::new();
        let mut total = 0i64;
        for provider in &self.providers {
            match provider.search(keyword, skip, take).await {
                Ok(result) => {
                    if result.support {
                        if let Some(t) = result.total {
                            total += t;
                        }
                        all_snapshots.extend(result.snapshots);
                    }
                }
                Err(e) => {
                    tracing::warn!(engine = provider.engine_name(), error = %e, "search failed");
                }
            }
        }
        Ok(SearchComicResult {
            support: true,
            snapshots: all_snapshots,
            total: Some(total),
        })
    }
}

impl Default for SearchEngine {
    fn default() -> Self {
        Self::new()
    }
}
