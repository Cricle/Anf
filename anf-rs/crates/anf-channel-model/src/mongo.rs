use anf_core::{ComicChapter, ComicPage};
use serde::{Deserialize, Serialize};

/// AnfComicEntityInfoOnly: base entity without chapters
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct AnfComicEntityInfoOnly {
    pub comic_url: String,
    pub name: String,
    #[serde(default)]
    pub descript: String,
    #[serde(default)]
    pub image_url: String,
    #[serde(default)]
    pub create_time: i64,
    #[serde(default)]
    pub update_time: i64,
}

/// AnfComicEntityTruck: entity with chapters
/// Matches C# AnfComicEntityTruck
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct AnfComicEntityTruck {
    #[serde(flatten)]
    pub info: AnfComicEntityInfoOnly,
    #[serde(default)]
    pub chapters: Vec<ComicChapter>,
}

/// WithPageChapter: chapter with pages and metadata
/// Matches C# WithPageChapter
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct WithPageChapter {
    pub target_url: String,
    pub title: String,
    #[serde(default)]
    pub pages: Vec<ComicPage>,
    #[serde(default)]
    pub create_time: i64,
    #[serde(default)]
    pub update_time: i64,
    #[serde(default)]
    pub ref_count: i64,
}
