use anf_core::{ComicSnapshot, NetworkAdapter, ProposalProvider};
use async_trait::async_trait;
use mlua::prelude::*;
use std::sync::Arc;

use crate::{lua_network, lua_snapshot};

pub struct LuaProposalProvider {
    lua: Lua,
    engine_name: String,
}

impl LuaProposalProvider {
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
impl ProposalProvider for LuaProposalProvider {
    fn engine_name(&self) -> &str {
        &self.engine_name
    }

    async fn get_proposal(&self, take: i32) -> anf_core::Result<Vec<ComicSnapshot>> {
        let globals = self.lua.globals();
        let func: LuaFunction = globals
            .get("get_proposal")
            .map_err(|_| anf_core::AnfError::Other("Lua script missing get_proposal()".into()))?;
        let result: LuaTable = func
            .call(take)
            .map_err(|e| anf_core::AnfError::Other(format!("get_proposal error: {e}")))?;

        lua_snapshot::parse_snapshots(result, &self.engine_name)
    }
}
