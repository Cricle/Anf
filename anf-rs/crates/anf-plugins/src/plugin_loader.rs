use crate::lua_condition::LuaCondition;
use anf_core::{ComicEngine, ComicSourceCondition};
use std::path::{Path, PathBuf};
use std::sync::Arc;

/// PluginLoader: scans a directory for `.lua` plugin files and registers them
/// as ComicSourceCondition into a ComicEngine.
pub struct PluginLoader {
    plugin_dir: PathBuf,
}

impl PluginLoader {
    pub fn new(plugin_dir: impl Into<PathBuf>) -> Self {
        Self {
            plugin_dir: plugin_dir.into(),
        }
    }

    /// Load all `.lua` files from the plugin directory and register them into the engine.
    /// Returns the number of successfully loaded plugins.
    pub fn load_all(&self, engine: &mut ComicEngine) -> usize {
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
            match self.load_plugin(path) {
                Ok(condition) => {
                    tracing::info!(name = condition.engine_name(), path = %path.display(), "loaded lua plugin");
                    engine.add(Arc::new(condition));
                    loaded += 1;
                }
                Err(e) => {
                    tracing::warn!(path = %path.display(), error = %e, "failed to load lua plugin");
                }
            }
        }
        loaded
    }

    fn load_plugin(&self, path: &Path) -> Result<LuaCondition, String> {
        LuaCondition::from_file(path).map_err(|e| e.to_string())
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_plugin_dir_not_found() {
        let loader = PluginLoader::new("/nonexistent");
        let network = Arc::new(anf_core::ReqwestAdapter::with_default());
        let mut engine = ComicEngine::new(network);
        assert_eq!(loader.load_all(&mut engine), 0);
    }
}
