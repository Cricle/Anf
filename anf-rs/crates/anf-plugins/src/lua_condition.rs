use crate::lua_provider::LuaProvider;
use anf_core::{
    ComicEntity, ComicPage, ComicSourceCondition, ComicSourceContext, ComicSourceProvider,
    NetworkAdapter,
};
use mlua::prelude::*;
use std::sync::Arc;

/// LuaCondition: implements ComicSourceCondition by delegating to a Lua script.
///
/// The Lua script must define:
///   - engine_name()              -> string
///   - address()                  -> string
///   - condition(url, host)       -> bool
///   - get_chapters(url)          -> table
///   - get_pages(url)             -> table
///   - get_image(url)             -> string
pub struct LuaCondition {
    script: String,
    engine_name_cache: String,
    address_cache: String,
    favicon_cache: String,
    order_cache: i32,
}

impl LuaCondition {
    pub fn from_file(path: &std::path::Path) -> LuaResult<Self> {
        let script = std::fs::read_to_string(path)
            .map_err(|e| LuaError::runtime(format!("failed to read {}: {e}", path.display())))?;
        Self::from_script(&script)
    }

    pub fn from_script(script: &str) -> LuaResult<Self> {
        let lua = Lua::new();
        lua.load(script).exec()?;

        let globals = lua.globals();
        let engine_name: String = globals
            .get("engine_name")
            .map_err(|_| LuaError::runtime("Lua plugin missing engine_name()"))?;
        let address: String = globals
            .get("address")
            .map_err(|_| LuaError::runtime("Lua plugin missing address()"))?;
        let favicon: String = globals
            .get::<LuaFunction>("favicon_address")
            .ok()
            .and_then(|f| f.call::<String>(()).ok())
            .unwrap_or_else(|| format!("{address}/favicon.ico"));
        let order: i32 = globals
            .get::<LuaFunction>("order")
            .ok()
            .and_then(|f| f.call::<i32>(()).ok())
            .unwrap_or(0);

        Ok(Self {
            script: script.to_string(),
            engine_name_cache: engine_name,
            address_cache: address,
            favicon_cache: favicon,
            order_cache: order,
        })
    }
}

impl ComicSourceCondition for LuaCondition {
    fn engine_name(&self) -> &str {
        &self.engine_name_cache
    }
    fn address(&self) -> &str {
        &self.address_cache
    }
    fn favicon_address(&self) -> &str {
        &self.favicon_cache
    }
    fn order(&self) -> i32 {
        self.order_cache
    }

    fn condition(&self, ctx: &ComicSourceContext) -> bool {
        let lua = Lua::new();
        if lua.load(&self.script).exec().is_err() {
            return false;
        }
        let globals = lua.globals();
        let func: LuaFunction = match globals.get("condition") {
            Ok(f) => f,
            Err(_) => return false,
        };
        func.call((ctx.source.as_str(), ctx.host.as_deref().unwrap_or("")))
            .unwrap_or(false)
    }

    fn create_provider(&self, network: Arc<dyn NetworkAdapter>) -> Arc<dyn ComicSourceProvider> {
        match LuaProvider::new(network, &self.script) {
            Ok(p) => Arc::new(p),
            Err(e) => {
                tracing::error!(engine = %self.engine_name_cache, error = %e, "failed to create lua provider");
                Arc::new(NullProvider)
            }
        }
    }
}

struct NullProvider;

#[async_trait::async_trait]
impl ComicSourceProvider for NullProvider {
    async fn get_chapters(&self, _: &str) -> anf_core::Result<ComicEntity> {
        Err(anf_core::AnfError::Other("null lua provider".into()))
    }
    async fn get_pages(&self, _: &str) -> anf_core::Result<Vec<ComicPage>> {
        Err(anf_core::AnfError::Other("null lua provider".into()))
    }
    async fn get_image_stream(&self, _: &str) -> anf_core::Result<bytes::Bytes> {
        Err(anf_core::AnfError::Other("null lua provider".into()))
    }
}
