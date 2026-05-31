use anf_core::{ComicSnapshot, ComicSource};
use mlua::prelude::*;

/// Parse a Lua table sequence of snapshots into Vec<ComicSnapshot>.
pub fn parse_snapshots(tbl: LuaTable, engine_name: &str) -> anf_core::Result<Vec<ComicSnapshot>> {
    let mut snapshots = Vec::new();
    for pair in tbl.sequence_values::<LuaTable>() {
        let s = pair.map_err(|e| anf_core::AnfError::Other(format!("snapshot parse: {e}")))?;
        let target_url: String = s.get("target_url").unwrap_or_default();
        let name: String = s.get("name").unwrap_or_default();
        let author: String = s.get("author").unwrap_or_default();
        let image_uri: String = s.get("image_uri").unwrap_or_default();
        let descript: String = s.get("descript").unwrap_or_default();
        let source_name: String = s.get("source_name").unwrap_or_else(|_| engine_name.to_string());

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
