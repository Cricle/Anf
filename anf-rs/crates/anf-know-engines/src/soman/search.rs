use anf_core::{
    ComicSnapshot, ComicSource, JsonVisitor, NetworkAdapter, RequestSettings, SearchComicResult,
    SearchProvider,
};
use async_trait::async_trait;
use std::sync::Arc;

pub struct SomanSearchProvider {
    network: Arc<dyn NetworkAdapter>,
}
impl SomanSearchProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl SearchProvider for SomanSearchProvider {
    fn engine_name(&self) -> &str {
        "Soman"
    }

    async fn search(
        &self,
        keyword: &str,
        skip: i32,
        take: i32,
    ) -> anf_core::Result<SearchComicResult> {
        let page = if skip > 0 && skip > take {
            skip / take
        } else {
            1
        };
        let now = chrono::Utc::now();
        let ts = now.timestamp();
        let url = format!("http://api.soman.com/soman.ashx?action=getsomancomics2&pageindex={page}&pagesize={take}&keyword={}&time={ts}", urlencoding::encode(keyword));
        let settings = RequestSettings {
            address: url.clone(),
            referrer: Some("https://www.soman.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let jv = JsonVisitor::from_str(&str_)?;
        let total: i64 = jv.get("Total").to_string().parse().unwrap_or(0);
        let items = jv.get("Items").to_vec();
        let mut snapshots = Vec::new();
        for item in items {
            let comics = item.get("Comics").to_vec();
            if comics.is_empty() {
                continue;
            }
            let first = &comics[0];
            let sources: Vec<ComicSource> = comics
                .iter()
                .map(|c| ComicSource {
                    target_url: format!(
                        "{}{}",
                        c.get("Host"),
                        c.get("Url")
                    ),
                    name: c.get("Source").to_string(),
                })
                .collect();
            snapshots.push(ComicSnapshot {
                name: first.get("SomanId").to_string(),
                image_uri: first.get("PicUrl").to_string(),
                author: first.get("Author").to_string(),
                descript: first.get("Content").to_string(),
                target_url: url.clone(),
                sources,
            });
        }
        Ok(SearchComicResult {
            support: true,
            snapshots,
            total: Some(total),
        })
    }
}
