use anf_core::{
    ComicSnapshot, ComicSource, JsonVisitor, NetworkAdapter, RequestSettings, SearchComicResult,
    SearchProvider,
};
use async_trait::async_trait;
use std::sync::Arc;

pub struct KuaikanSearchProvider {
    network: Arc<dyn NetworkAdapter>,
}
impl KuaikanSearchProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl SearchProvider for KuaikanSearchProvider {
    fn engine_name(&self) -> &str {
        "Kuaikan"
    }
    async fn search(
        &self,
        keyword: &str,
        skip: i32,
        take: i32,
    ) -> anf_core::Result<SearchComicResult> {
        let page = if take > 0 { (skip / take).max(1) } else { 1 };
        let url = format!(
            "https://www.kuaikanmanhua.com/v1/search/topic?q={}&f={}&size={}",
            urlencoding::encode(keyword),
            page,
            take
        );
        let settings = RequestSettings {
            address: url.clone(),
            referrer: Some("https://www.kuaikanmanhua.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let jv = JsonVisitor::from_str(&str_)?;
        let hit = jv.get("data").get("hit").to_vec();
        let total = hit.len();
        let snapshots: Vec<ComicSnapshot> = hit
            .iter()
            .map(|item| {
                let id = item.get("id").to_string();
                ComicSnapshot {
                    name: item.get("title").to_string(),
                    author: item.get("user").get("nickname").to_string(),
                    image_uri: item.get("vertical_image_url").to_string(),
                    descript: item.get("description").to_string(),
                    target_url: url.clone(),
                    sources: vec![ComicSource {
                        target_url: format!("https://www.kuaikanmanhua.com/web/topic/{id}"),
                        name: "Kuaikan".into(),
                    }],
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
