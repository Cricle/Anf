use anf_core::{NetworkAdapter, SearchComicResult, SearchProvider};
use async_trait::async_trait;
use mlua::prelude::*;
use std::sync::Arc;

use crate::{lua_network, lua_snapshot};

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
}

#[async_trait]
impl SearchProvider for LuaSearchProvider {
    fn engine_name(&self) -> &str {
        &self.engine_name
    }

    async fn search(&self, keyword: &str, skip: i32, take: i32) -> anf_core::Result<SearchComicResult> {
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

        Ok(SearchComicResult {
            support,
            snapshots: lua_snapshot::parse_snapshots(snapshots_tbl, &self.engine_name)?,
            total,
        })
    }
}
