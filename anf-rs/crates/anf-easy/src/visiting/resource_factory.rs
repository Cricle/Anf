use async_trait::async_trait;
use std::sync::Arc;

use anf_core::{ComicEntity, ComicSourceProvider};

/// ResourceFactory trait: creates resources for visiting
/// Matches C# IResourceFactory<TResource>
#[async_trait]
pub trait ResourceFactory<T>: Send + Sync {
    async fn get(&self, address: &str) -> anf_core::Result<T>;
}

/// ResourceFactoryCreator trait
/// Matches C# IResourceFactoryCreator<TResource>
#[async_trait]
pub trait ResourceFactoryCreator<T>: Send + Sync {
    async fn create(
        &self,
        address: &str,
        provider: Arc<dyn ComicSourceProvider>,
        entity: &ComicEntity,
    ) -> anf_core::Result<Box<dyn ResourceFactory<T>>>;
}
