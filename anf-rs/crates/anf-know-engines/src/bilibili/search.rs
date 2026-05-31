use anf_core::{
    ComicSnapshot, ComicSource, JsonVisitor, NetworkAdapter, RequestSettings, SearchComicResult,
    SearchProvider,
};
use async_trait::async_trait;
use bytes::Bytes;
use std::sync::Arc;

pub struct BilibiliSearchProvider {
    network: Arc<dyn NetworkAdapter>,
}

impl BilibiliSearchProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl SearchProvider for BilibiliSearchProvider {
    fn engine_name(&self) -> &str {
        "Bilibili"
    }

    async fn search(
        &self,
        keyword: &str,
        skip: i32,
        take: i32,
    ) -> anf_core::Result<SearchComicResult> {
        let page = if take > 0 { (skip / take).max(1) } else { 1 };
        let body = serde_json::json!({
            "key_word": keyword,
            "page_num": page,
            "page_size": take
        })
        .to_string();
        let mut headers = std::collections::HashMap::new();
        headers.insert("Content-Type".into(), "application/json".into());
        headers.insert("User-Agent".into(), "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1".into());
        let settings = RequestSettings {
            address:
                "https://manga.bilibili.com/twirp/comic.v1.Comic/Search?device=pc&platform=web"
                    .into(),
            host: Some("manga.bilibili.com".into()),
            referrer: Some("https://manga.bilibili.com/".into()),
            method: Some("POST".into()),
            data: Some(Bytes::from(body)),
            headers: Some(headers),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let jv = JsonVisitor::from_str(&str_)?;
        let list = jv.get("data").get("list").to_vec();
        let total = list.len();
        let snapshots: Vec<ComicSnapshot> = list
            .iter()
            .map(|item| {
                let id = item.get("id").to_string();
                ComicSnapshot {
                    author: item
                        .get("author_name")
                        .to_vec()
                        .iter()
                        .map(|a| a.to_string())
                        .collect::<Vec<_>>()
                        .join(","),
                    image_uri: item.get("vertical_cover").to_string(),
                    name: item.get("org_title").to_string(),
                    target_url: format!("https://manga.bilibili.com/detail/mc{}", id),
                    sources: vec![ComicSource {
                        target_url: format!("https://manga.bilibili.com/detail/mc{}", id),
                        name: "Bilibili".into(),
                    }],
                    descript: String::new(),
                }
            })
            .collect();
        Ok(SearchComicResult {
            support: true,
            snapshots,
            total: Some(total as i64),
        })
    }
}
