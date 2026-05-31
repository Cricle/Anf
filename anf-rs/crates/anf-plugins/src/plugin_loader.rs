use crate::lua_condition::LuaCondition;
use crate::lua_proposal::LuaProposalProvider;
use crate::lua_search::LuaSearchProvider;
use anf_core::{ComicEngine, ComicSourceCondition, NetworkAdapter, ProposalDescription, ProposalEngine, SearchEngine};
use std::path::{Path, PathBuf};
use std::sync::Arc;

/// PluginLoader: scans a directory for `.lua` plugin files and registers them
/// as ComicSourceCondition, SearchProvider, and/or ProposalProvider.
pub struct PluginLoader {
    plugin_dir: PathBuf,
}

impl PluginLoader {
    pub fn new(plugin_dir: impl Into<PathBuf>) -> Self {
        Self {
            plugin_dir: plugin_dir.into(),
        }
    }

    /// Load all `.lua` files from the plugin directory and register them into the engines.
    /// Returns the number of successfully loaded plugins.
    pub fn load_all(
        &self,
        engine: &mut ComicEngine,
        search_engine: &mut SearchEngine,
        proposal_engine: &mut ProposalEngine,
        network: Arc<dyn NetworkAdapter>,
    ) -> usize {
        let pattern = self.plugin_dir.join("*.lua");
        let pattern_str = pattern.to_string_lossy();

        let paths: Vec<PathBuf> = match glob::glob(&pattern_str) {
            Ok(paths) => paths.filter_map(Result::ok).collect(),
            Err(e) => {
                tracing::error!(error = %e, "failed to glob plugin directory");
                return 0;
            }
        };

        let mut loaded = 0;
        for path in &paths {
            match self.load_plugin(path, engine, search_engine, proposal_engine, network.clone()) {
                Ok(count) => {
                    loaded += count;
                }
                Err(e) => {
                    tracing::warn!(path = %path.display(), error = %e, "failed to load lua plugin");
                }
            }
        }
        loaded
    }

    fn load_plugin(
        &self,
        path: &Path,
        engine: &mut ComicEngine,
        search_engine: &mut SearchEngine,
        proposal_engine: &mut ProposalEngine,
        network: Arc<dyn NetworkAdapter>,
    ) -> Result<usize, String> {
        let script = std::fs::read_to_string(path)
            .map_err(|e| format!("failed to read {}: {e}", path.display()))?;

        // Try to detect engine_name from the script
        let engine_name = Self::extract_engine_name(&script).unwrap_or_else(|| {
            path.file_stem()
                .and_then(|s| s.to_str())
                .unwrap_or("unknown")
                .to_string()
        });

        let mut count = 0;

        // Check if script defines condition() → register as ComicSourceCondition
        if Self::has_function(&script, "condition") {
            match LuaCondition::from_script(&script) {
                Ok(condition) => {
                    tracing::info!(name = condition.engine_name(), path = %path.display(), "loaded lua comic condition");
                    engine.add(Arc::new(condition));
                    count += 1;
                }
                Err(e) => {
                    tracing::warn!(path = %path.display(), error = %e, "failed to load lua condition");
                }
            }
        }

        // Check if script defines search() → register as SearchProvider
        if Self::has_function(&script, "search") {
            match LuaSearchProvider::new(network.clone(), &script, &engine_name) {
                Ok(provider) => {
                    tracing::info!(name = %engine_name, path = %path.display(), "loaded lua search provider");
                    search_engine.add(Arc::new(provider));
                    count += 1;
                }
                Err(e) => {
                    tracing::warn!(path = %path.display(), error = %e, "failed to load lua search provider");
                }
            }
        }

        // Check if script defines get_proposal() → register as ProposalProvider
        if Self::has_function(&script, "get_proposal") {
            match LuaProposalProvider::new(network.clone(), &script, &engine_name) {
                Ok(provider) => {
                    tracing::info!(name = %engine_name, path = %path.display(), "loaded lua proposal provider");
                    proposal_engine.add(
                        ProposalDescription {
                            name: engine_name.clone(),
                            provider_name: engine_name.clone(),
                        },
                        Arc::new(provider),
                    );
                    count += 1;
                }
                Err(e) => {
                    tracing::warn!(path = %path.display(), error = %e, "failed to load lua proposal provider");
                }
            }
        }

        if count == 0 {
            return Err("lua script defines no recognized functions (condition/search/get_proposal)".into());
        }

        Ok(count)
    }

    /// Check if a Lua script defines a global function with the given name.
    fn has_function(script: &str, fn_name: &str) -> bool {
        // Quick heuristic: look for "function fn_name(" in the script
        script.contains(&format!("function {fn_name}("))
    }

    /// Extract engine_name from a Lua script by evaluating it and reading the global.
    fn extract_engine_name(script: &str) -> Option<String> {
        let lua = mlua::Lua::new();
        lua.load(script).exec().ok()?;
        let globals = lua.globals();
        globals.get::<String>("engine_name").ok()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_plugin_dir_not_found() {
        let loader = PluginLoader::new("/nonexistent");
        let network = Arc::new(anf_core::ReqwestAdapter::with_default());
        let mut engine = ComicEngine::new(network.clone());
        let mut search = SearchEngine::new();
        let mut proposal = ProposalEngine::new();
        assert_eq!(loader.load_all(&mut engine, &mut search, &mut proposal, network), 0);
    }
}
