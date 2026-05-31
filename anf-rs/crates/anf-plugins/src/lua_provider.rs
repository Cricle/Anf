use anf_core::{
    ComicChapter, ComicEntity, ComicInfo, ComicPage, ComicSourceProvider, NetworkAdapter,
};
use async_trait::async_trait;
use bytes::Bytes;
use mlua::prelude::*;
use std::sync::Arc;

use crate::lua_network;

/// LuaProvider: implements ComicSourceProvider by delegating to a Lua script.
///
/// The Lua script must define:
///   - get_chapters(url) -> table
///   - get_pages(url)    -> table
///   - get_image(url)    -> string (raw bytes)
///
/// The `http` and `html` globals are injected for network/HTML operations.
pub struct LuaProvider {
    lua: Lua,
}

impl LuaProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>, script: &str) -> LuaResult<Self> {
        let lua = Lua::new();
        lua_network::register_network(&lua, network)?;
        register_html_helper(&lua)?;
        lua.load(script).exec()?;
        Ok(Self { lua })
    }

    fn call_get_chapters(&self, target_url: &str) -> anf_core::Result<ComicEntity> {
        let globals = self.lua.globals();
        let func: LuaFunction = globals
            .get("get_chapters")
            .map_err(|_| anf_core::AnfError::Other("Lua script missing get_chapters()".into()))?;
        let result: LuaTable = func
            .call(target_url)
            .map_err(|e| anf_core::AnfError::Other(format!("get_chapters error: {e}")))?;

        let name: String = result.get("name").unwrap_or_default();
        let descript: String = result.get("descript").unwrap_or_default();
        let image_url: String = result.get("image_url").unwrap_or_default();
        let chapters_tbl: LuaTable = result
            .get("chapters")
            .map_err(|e| anf_core::AnfError::Other(format!("missing chapters: {e}")))?;

        let mut chapters = Vec::new();
        for pair in chapters_tbl.sequence_values::<LuaTable>() {
            let ch = pair.map_err(|e| anf_core::AnfError::Other(format!("chapter parse: {e}")))?;
            chapters.push(ComicChapter {
                target_url: ch.get("target_url").unwrap_or_default(),
                title: ch.get("title").unwrap_or_default(),
            });
        }

        Ok(ComicEntity {
            info: ComicInfo {
                comic_url: target_url.to_string(),
                name,
                descript,
                image_url,
            },
            chapters,
        })
    }

    fn call_get_pages(&self, target_url: &str) -> anf_core::Result<Vec<ComicPage>> {
        let globals = self.lua.globals();
        let func: LuaFunction = globals
            .get("get_pages")
            .map_err(|_| anf_core::AnfError::Other("Lua script missing get_pages()".into()))?;
        let result: LuaTable = func
            .call(target_url)
            .map_err(|e| anf_core::AnfError::Other(format!("get_pages error: {e}")))?;

        let mut pages = Vec::new();
        for pair in result.sequence_values::<LuaTable>() {
            let p = pair.map_err(|e| anf_core::AnfError::Other(format!("page parse: {e}")))?;
            pages.push(ComicPage {
                name: p.get("name").unwrap_or_default(),
                target_url: p.get("target_url").unwrap_or_default(),
            });
        }
        Ok(pages)
    }

    fn call_get_image(&self, target_url: &str) -> anf_core::Result<Bytes> {
        let globals = self.lua.globals();
        let func: LuaFunction = globals
            .get("get_image")
            .map_err(|_| anf_core::AnfError::Other("Lua script missing get_image()".into()))?;
        let result: LuaString = func
            .call(target_url)
            .map_err(|e| anf_core::AnfError::Other(format!("get_image error: {e}")))?;
        Ok(Bytes::from(result.as_bytes().to_vec()))
    }
}

#[async_trait]
impl ComicSourceProvider for LuaProvider {
    async fn get_chapters(&self, target_url: &str) -> anf_core::Result<ComicEntity> {
        let url = target_url.to_string();
        // Lua is single-threaded; call synchronously
        // Synchronous call — Lua is single-threaded anyway
        self.call_get_chapters(&url)
    }

    async fn get_pages(&self, target_url: &str) -> anf_core::Result<Vec<ComicPage>> {
        self.call_get_pages(target_url)
    }

    async fn get_image_stream(&self, target_url: &str) -> anf_core::Result<Bytes> {
        self.call_get_image(target_url)
    }
}

/// Register `html` global with HTML parsing helpers backed by scraper.
pub fn register_html_helper_static(lua: &Lua) -> LuaResult<()> {
    register_html_helper(lua)
}

fn register_html_helper(lua: &Lua) -> LuaResult<()> {
    let html = lua.create_table()?;

    // html.parse(html_str) -> table { _html = str }
    let parse_fn = lua.create_function(|lua, html_str: String| {
        let tbl = lua.create_table()?;
        tbl.set("_html", html_str)?;
        Ok(tbl)
    })?;
    html.set("parse", parse_fn)?;

    // html.select_one(html_str, selector) -> string|nil
    let select_one = lua.create_function(|_lua, (html_str, selector): (String, String)| {
        let doc = scraper::Html::parse_document(&html_str);
        let sel = scraper::Selector::parse(&selector)
            .map_err(|e| LuaError::runtime(format!("bad selector: {e}")))?;
        Ok(doc.select(&sel).next().map(|n| n.html()))
    })?;
    html.set("select_one", select_one)?;

    // html.select_all(html_str, selector) -> {string, ...}
    let select_all = lua.create_function(|_lua, (html_str, selector): (String, String)| {
        let doc = scraper::Html::parse_document(&html_str);
        let sel = scraper::Selector::parse(&selector)
            .map_err(|e| LuaError::runtime(format!("bad selector: {e}")))?;
        let results: Vec<String> = doc.select(&sel).map(|n| n.html()).collect();
        Ok(results)
    })?;
    html.set("select_all", select_all)?;

    // html.text(html_str, selector) -> string|nil
    let text_fn = lua.create_function(|_lua, (html_str, selector): (String, String)| {
        let doc = scraper::Html::parse_document(&html_str);
        let sel = scraper::Selector::parse(&selector)
            .map_err(|e| LuaError::runtime(format!("bad selector: {e}")))?;
        Ok(doc
            .select(&sel)
            .next()
            .map(|n| n.text().collect::<String>()))
    })?;
    html.set("text", text_fn)?;

    // html.attr(html_str, selector, attr_name) -> string|nil
    let attr_fn = lua.create_function(
        |_lua, (html_str, selector, attr): (String, String, String)| {
            let doc = scraper::Html::parse_document(&html_str);
            let sel = scraper::Selector::parse(&selector)
                .map_err(|e| LuaError::runtime(format!("bad selector: {e}")))?;
            Ok(doc
                .select(&sel)
                .next()
                .and_then(|n| n.value().attr(&attr).map(|s| s.to_string())))
        },
    )?;
    html.set("attr", attr_fn)?;

    lua.globals().set("html", html)?;
    Ok(())
}
