use anf_core::{ComicSnapshot, ComicSource, NetworkAdapter, ProposalProvider};
use async_trait::async_trait;
use mlua::prelude::*;
use std::sync::Arc;

use crate::lua_network;

/// LuaProposalProvider: implements ProposalProvider by delegating to a Lua script.
///
/// The Lua script must define:
///   - get_proposal(take) -> { {name=, target_url=, ...}, ... }
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

    fn call_get_proposal(&self, take: i32) -> anf_core::Result<Vec<ComicSnapshot>> {
        let globals = self.lua.globals();
        let func: LuaFunction = globals
            .get("get_proposal")
            .map_err(|_| anf_core::AnfError::Other("Lua script missing get_proposal()".into()))?;
        let result: LuaTable = func
            .call(take)
            .map_err(|e| anf_core::AnfError::Other(format!("get_proposal error: {e}")))?;

        let mut snapshots = Vec::new();
        for pair in result.sequence_values::<LuaTable>() {
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

        Ok(snapshots)
    }
}

#[async_trait]
impl ProposalProvider for LuaProposalProvider {
    fn engine_name(&self) -> &str {
        &self.engine_name
    }

    async fn get_proposal(&self, take: i32) -> anf_core::Result<Vec<ComicSnapshot>> {
        self.call_get_proposal(take)
    }
}
