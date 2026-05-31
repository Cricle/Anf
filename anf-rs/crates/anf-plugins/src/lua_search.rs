use anf_core::{ComicSnapshot, ComicSource, NetworkAdapter, SearchComicResult, SearchProvider};
use async_trait::async_trait;
use mlua::prelude::*;
use std::sync::Arc;

use crate::lua_network;

/// LuaSearchProvider: implements SearchProvider by delegating to a Lua script.
///
/// The Lua script must define:
///   - search(keyword, skip, take) -> table { support=bool, snapshots={...}, total=number }
pub struct LuaSearchProvider {
    lua: Lua,
    engine_name: String,
}

impl LuaSearchProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>, script: &str, engine_name: &str) -> LuaResult<Self> {
        let lua = Lua::new();
        lua_network::register_network(&lua, network)?;
        crate::lua_provider::register_html_helper_static(&lua)?;
        lua.load(script).exec()?;
        Ok(Self {
            lua,
            engine_name: engine_name.to_string(),
        })
    }

    fn call_search(&self, keyword: &str, skip: i32, take: i32) -> anf_core::Result<SearchComicResult> {
        let globals = self.lua.globals();
        let func: LuaFunction = globals
            .get("search")
            .map_err(|_| anf_core::AnfError::Other("Lua script missing search()".into()))?;
        let result: LuaTable = func
            .call((keyword, skip, take))
            .map_err(|e| anf_core::AnfError::Other(format!("search error: {e}")))?;

        let support: bool = result.get("support").unwrap_or(true);
        let total: Option<i64> = result.get("total").ok();

        let snapshots_tbl: LuaTable = result
            .get("snapshots")
            .map_err(|e| anf_core::AnfError::Other(format!("missing snapshots: {e}")))?;

        let mut snapshots = Vec::new();
        for pair in snapshots_tbl.sequence_values::<LuaTable>() {
            let s = pair.map_err(|e| anf_core::AnfError::Other(format!("snapshot parse: {e}")))?;
            let target_url: String = s.get("target_url").unwrap_or_default();
            let name: String = s.get("name").unwrap_or_default();
            let author: String = s.get("author").unwrap_or_default();
            let image_uri: String = s.get("image_uri").unwrap_or_default();
            let descript: String = s.get("descript").unwrap_or_default();
            let source_name: String = s.get("source_name").unwrap_or_else(|_| self.engine_name.clone());

            snapshots.push(ComicSnapshot {
                name,
                author,
                image_uri,
                target_url: target_url.clone(),
                sources: vec![ComicSource {
                    target_url,
                    name: source_name,
                }],
                descript,
            });
        }

        Ok(SearchComicResult {
            support,
            snapshots,
            total,
        })
    }
}

#[async_trait]
impl SearchProvider for LuaSearchProvider {
    fn engine_name(&self) -> &str {
        &self.engine_name
    }

    async fn search(&self, keyword: &str, skip: i32, take: i32) -> anf_core::Result<SearchComicResult> {
        self.call_search(keyword, skip, take)
    }
}
