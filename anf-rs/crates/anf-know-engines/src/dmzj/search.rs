use anf_core::{
    ComicSnapshot, ComicSource, NetworkAdapter, RequestSettings, SearchComicResult, SearchProvider,
};
use async_trait::async_trait;
use scraper::{Html, Selector};
use std::sync::Arc;

pub struct DmzjSearchProvider {
    network: Arc<dyn NetworkAdapter>,
}

impl DmzjSearchProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl SearchProvider for DmzjSearchProvider {
    fn engine_name(&self) -> &str {
        "DMZJ"
    }

    async fn search(
        &self,
        keyword: &str,
        skip: i32,
        take: i32,
    ) -> anf_core::Result<SearchComicResult> {
        let url = format!(
            "https://s.dmzj.com/search?keyword={}",
            urlencoding::encode(keyword)
        );
        let settings = RequestSettings {
            address: url.clone(),
            referrer: Some("https://www.dmzj.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);

        let item_sel = Selector::parse("div.search-result-list div.search-result-item").unwrap();
        let items: Vec<_> = doc
            .select(&item_sel)
            .skip(skip as usize)
            .take(take as usize)
            .collect();
        let total = items.len();
        let mut snapshots = Vec::new();

        for item in items {
            let a_sel = Selector::parse("a").unwrap();
            let (title, href) = if let Some(a) = item.select(&a_sel).next() {
                (
                    a.text().collect::<String>().trim().to_string(),
                    a.value().attr("href").unwrap_or("").to_string(),
                )
            } else {
                continue;
            };

            let img_sel = Selector::parse("img").unwrap();
            let cover = item
                .select(&img_sel)
                .next()
                .and_then(|n| n.value().attr("src"))
                .unwrap_or("")
                .to_string();

            let author_sel = Selector::parse("span.author").unwrap();
            let author = item
                .select(&author_sel)
                .next()
                .map(|n| n.text().collect::<String>().trim().to_string())
                .unwrap_or_default();

            let href = if !href.starts_with("http") {
                format!("https://manhua.dmzj.com{href}")
            } else {
                href
            };

            snapshots.push(ComicSnapshot {
                name: title,
                author,
                image_uri: cover,
                target_url: href.clone(),
                sources: vec![ComicSource {
                    target_url: href,
                    name: "DMZJ".into(),
                }],
                descript: String::new(),
            });
        }

        Ok(SearchComicResult {
            support: true,
            snapshots,
            total: Some(total as i64),
        })
    }
}
