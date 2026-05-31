use anf_core::{
    ComicSnapshot, ComicSource, JsonVisitor, NetworkAdapter, ProposalProvider, RequestSettings,
};
use async_trait::async_trait;
use bytes::Bytes;
use std::sync::Arc;

pub struct BilibiliProposalProvider {
    network: Arc<dyn NetworkAdapter>,
}

impl BilibiliProposalProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl ProposalProvider for BilibiliProposalProvider {
    fn engine_name(&self) -> &str {
        "Bilibili"
    }

    async fn get_proposal(&self, take: i32) -> anf_core::Result<Vec<ComicSnapshot>> {
        let mut headers = std::collections::HashMap::new();
        headers.insert("Content-Type".into(), "application/json".into());
        let settings = RequestSettings {
            address: "https://manga.bilibili.com/twirp/comic.v1.Comic/HomeRecommend?device=pc&platform=web".into(),
            referrer: Some("https://manga.bilibili.com/".into()),
            method: Some("POST".into()),
            data: Some(Bytes::from(r#"{"page_num":4,"seed":"0"}"#)),
            headers: Some(headers),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let jv = JsonVisitor::from_str(&str_)?;
        let list = jv.get("data").get("list").to_vec();
        let mut snapshots = Vec::new();
        for (i, tk) in list.iter().enumerate() {
            if i >= take as usize {
                break;
            }
            let id = tk.get("comic_id").to_string();
            let authors: Vec<String> = tk
                .get("authors")
                .to_vec()
                .iter()
                .map(|a| a.to_string())
                .collect();
            snapshots.push(ComicSnapshot {
                name: tk.get("title").to_string(),
                author: authors.join("-"),
                image_uri: tk.get("vertical_cover").to_string(),
                target_url: format!("https://manga.bilibili.com/detail/{}", id),
                sources: vec![ComicSource {
                    target_url: format!("https://manga.bilibili.com/detail/{}", id),
                    name: "Bilibili".into(),
                }],
                descript: String::new(),
            });
        }
        Ok(snapshots)
    }
}
