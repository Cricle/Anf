use anf_core::{
    ComicSnapshot, ComicSource, JsonVisitor, NetworkAdapter, RequestSettings, SearchComicResult,
    SearchProvider,
};
use async_trait::async_trait;
use std::sync::Arc;

pub struct QimiaoSearchProvider {
    network: Arc<dyn NetworkAdapter>,
}
impl QimiaoSearchProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl SearchProvider for QimiaoSearchProvider {
    fn engine_name(&self) -> &str {
        "Qimiao"
    }

    async fn search(
        &self,
        keyword: &str,
        skip: i32,
        take: i32,
    ) -> anf_core::Result<SearchComicResult> {
        let page = if take > 0 { (skip / take).max(1) } else { 1 };
        let url = format!(
            "https://www.qimiaomh.com/api/search?keyword={}&page={}&pageSize={}",
            urlencoding::encode(keyword),
            page,
            take
        );
        let settings = RequestSettings {
            address: url.clone(),
            referrer: Some("https://www.qimiaomh.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let jv = JsonVisitor::from_str(&str_)?;
        let data = jv.get("data").get("list").to_vec();
        let total = data.len();
        let snapshots: Vec<ComicSnapshot> = data
            .iter()
            .map(|item| ComicSnapshot {
                name: item.get("name").to_string(),
                author: item.get("author").to_string(),
                image_uri: item.get("cover").to_string(),
                descript: item.get("desc").to_string(),
                target_url: url.clone(),
                sources: vec![ComicSource {
                    target_url: format!(
                        "https://www.qimiaomh.com/comic/{}",
                        item.get("id")
                    ),
                    name: "Qimiao".into(),
                }],
            })
            .collect();
        Ok(SearchComicResult {
            support: true,
            snapshots,
            total: Some(total as i64),
        })
    }
}
