use serde::{Deserialize, Serialize};

// ── ComicRef: base with TargetUrl ──────────────────────────────

#[derive(Debug, Clone, Serialize, Deserialize, PartialEq, Eq, Hash)]
pub struct ComicRef {
    pub target_url: String,
}

// ── ComicChapter ───────────────────────────────────────────────

#[derive(Debug, Clone, Serialize, Deserialize, PartialEq, Eq)]
pub struct ComicChapter {
    pub target_url: String,
    pub title: String,
}

// ── ComicPage ──────────────────────────────────────────────────

#[derive(Debug, Clone, Serialize, Deserialize, PartialEq, Eq)]
pub struct ComicPage {
    pub name: String,
    pub target_url: String,
}

// ── ComicInfo / ComicEntity ────────────────────────────────────

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ComicInfo {
    pub comic_url: String,
    pub name: String,
    #[serde(default)]
    pub descript: String,
    #[serde(default)]
    pub image_url: String,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ComicEntity {
    #[serde(flatten)]
    pub info: ComicInfo,
    #[serde(default)]
    pub chapters: Vec<ComicChapter>,
}

// Convenience accessors
impl ComicEntity {
    pub fn comic_url(&self) -> &str {
        &self.info.comic_url
    }
    pub fn name(&self) -> &str {
        &self.info.name
    }
    pub fn descript(&self) -> &str {
        &self.info.descript
    }
    pub fn image_url(&self) -> &str {
        &self.info.image_url
    }
}

// ── ChapterWithPage / ComicDetail ──────────────────────────────

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ChapterWithPage {
    pub chapter: ComicChapter,
    pub pages: Vec<ComicPage>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ComicDetail {
    pub entity: ComicEntity,
    pub chapters: Vec<ChapterWithPage>,
}

// ── ComicSource / ComicSnapshot ────────────────────────────────

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ComicSource {
    pub target_url: String,
    pub name: String,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ComicSnapshot {
    pub target_url: String,
    pub name: String,
    #[serde(default)]
    pub author: String,
    #[serde(default)]
    pub image_uri: String,
    #[serde(default)]
    pub descript: String,
    #[serde(default)]
    pub sources: Vec<ComicSource>,
}

// ── SearchComicResult ──────────────────────────────────────────

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct SearchComicResult {
    #[serde(default)]
    pub support: bool,
    #[serde(default)]
    pub snapshots: Vec<ComicSnapshot>,
    pub total: Option<i64>,
}

// ── EngineDescript ─────────────────────────────────────────────

#[derive(Debug, Clone, Default, Serialize, Deserialize)]
pub struct EngineDescript {
    pub name: Option<String>,
    pub url: Option<String>,
    pub descript: Option<String>,
    #[serde(flatten)]
    pub extra: std::collections::HashMap<String, String>,
}

// ── EngineDescriptConst ────────────────────────────────────────

pub mod engine_descript_const {
    pub const NAME: &str = "name";
    pub const URL: &str = "url";
    pub const DESCRIPT: &str = "descript";
}

// ── ComicSourceContext ─────────────────────────────────────────

#[derive(Debug, Clone)]
pub struct ComicSourceContext {
    pub source: String,
    pub host: Option<String>,
}

impl ComicSourceContext {
    pub fn new(source: impl Into<String>) -> crate::Result<Self> {
        let source = source.into();
        if source.is_empty() {
            return Err(crate::AnfError::InvalidUrl("source cannot be empty".into()));
        }
        let host = extract_host(&source);
        Ok(Self { source, host })
    }
}

fn extract_host(url: &str) -> Option<String> {
    // Fast host extraction matching C# UrlHelper.FastGetHost
    let bytes = url.as_bytes();
    let len = bytes.len();
    let mut start = 0usize;
    let mut end = len;
    let mut i = 0;
    while i < len {
        let c = bytes[i];
        if c == b'/' || c == b'?' {
            end = i;
            break;
        } else if c == b':' && (len - i) > 3 && bytes[i + 1] == b'/' && bytes[i + 2] == b'/' {
            start = i + 3;
            i = start;
        }
        i += 1;
    }
    if start < end && end <= len {
        Some(url[start..end].to_string())
    } else {
        None
    }
}

// ── ChapterAnalysis contexts ───────────────────────────────────

#[derive(Debug, Clone)]
pub struct ComicAnalysingContext {
    pub address: String,
}

#[derive(Debug, Clone)]
pub struct ComicAnalysedContext {
    pub address: String,
    pub entity: ComicEntity,
}

#[derive(Debug, Clone)]
pub struct ChapterAnalysingContext {
    pub address: String,
    pub chapter: ComicChapter,
    pub index: usize,
    pub entity: ComicEntity,
}

#[derive(Debug, Clone)]
pub struct ChapterAnalysedContext {
    pub address: String,
    pub chapter: ComicChapter,
    pub index: usize,
    pub entity: ComicEntity,
    pub chapter_with_page: ChapterWithPage,
}
