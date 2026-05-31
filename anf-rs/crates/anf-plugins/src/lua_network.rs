use anf_core::{NetworkAdapter, RequestSettings};
use bytes::Bytes;
use mlua::prelude::*;
use std::sync::Arc;

/// Register the `http` global table into the Lua VM.
///
/// Provides:
///   http.get_string(url [, opts]) -> string
///   http.get(url [, opts])        -> bytes (LuaString)
///   http.post(url, body [, opts]) -> string
pub fn register_network(lua: &Lua, network: Arc<dyn NetworkAdapter>) -> LuaResult<()> {
    let http = lua.create_table()?;

    // ── http.get_string(url [, opts]) ──────────────────────
    {
        let net = network.clone();
        let f = lua.create_function(move |_lua, args: LuaMultiValue| {
            let url: String = args
                .get(0)
                .and_then(|v| v.as_str())
                .map(|s| s.to_string())
                .ok_or_else(|| LuaError::runtime("http.get_string: url required"))?;
            let settings = build_settings(&url, &args);
            let rt = tokio::runtime::Handle::current();
            let s = rt
                .block_on(async { net.get_string(&settings).await })
                .map_err(|e| LuaError::runtime(format!("network: {e}")))?;
            Ok(s)
        })?;
        http.set("get_string", f)?;
    }

    // ── http.get(url [, opts]) ─────────────────────────────
    {
        let net = network.clone();
        let f = lua.create_function(move |_lua, args: LuaMultiValue| {
            let url: String = args
                .get(0)
                .and_then(|v| v.as_str())
                .map(|s| s.to_string())
                .ok_or_else(|| LuaError::runtime("http.get: url required"))?;
            let settings = build_settings(&url, &args);
            let rt = tokio::runtime::Handle::current();
            let data = rt
                .block_on(async { net.get_stream(&settings).await })
                .map_err(|e| LuaError::runtime(format!("network: {e}")))?;
            Ok(_lua.create_string(&data)?)
        })?;
        http.set("get", f)?;
    }

    // ── http.post(url, body [, opts]) ──────────────────────
    {
        let net = network.clone();
        let f = lua.create_function(move |_lua, args: LuaMultiValue| {
            let url: String = args
                .get(0)
                .and_then(|v| v.as_str())
                .map(|s| s.to_string())
                .ok_or_else(|| LuaError::runtime("http.post: url required"))?;
            let body: String = args
                .get(1)
                .and_then(|v| v.as_str())
                .map(|s| s.to_string())
                .ok_or_else(|| LuaError::runtime("http.post: body required"))?;
            let mut settings = build_settings(&url, &args);
            settings.method = Some("POST".into());
            settings.data = Some(Bytes::from(body));
            let rt = tokio::runtime::Handle::current();
            let s = rt
                .block_on(async { net.get_string(&settings).await })
                .map_err(|e| LuaError::runtime(format!("network: {e}")))?;
            Ok(s)
        })?;
        http.set("post", f)?;
    }

    lua.globals().set("http", http)?;
    Ok(())
}

fn build_settings(url: &str, args: &LuaMultiValue) -> RequestSettings {
    let mut settings = RequestSettings {
        address: url.to_string(),
        ..Default::default()
    };
    if let Some(LuaValue::Table(opts)) = args.get(2) {
        if let Ok(referrer) = opts.get::<String>("referrer") {
            settings.referrer = Some(referrer);
        }
        if let Ok(host) = opts.get::<String>("host") {
            settings.host = Some(host);
        }
        if let Ok(method) = opts.get::<String>("method") {
            settings.method = Some(method);
        }
        if let Ok(hdrs) = opts.get::<LuaTable>("headers") {
            let mut headers = std::collections::HashMap::new();
            for pair in hdrs.pairs::<String, String>() {
                if let Ok((k, v)) = pair {
                    headers.insert(k, v);
                }
            }
            settings.headers = Some(headers);
        }
    }
    settings
}
