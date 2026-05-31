use anf_core::{
    ComicSnapshot, ComicSource, NetworkAdapter, RequestSettings, SearchComicResult, SearchProvider,
};
use async_trait::async_trait;
use scraper::{Html, Selector};
use std::sync::Arc;

pub struct MangabzSearchProvider {
    network: Arc<dyn NetworkAdapter>,
    base_url: String,
}
impl MangabzSearchProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self {
            network,
            base_url: "http://www.mangabz.com".into(),
        }
    }

    pub fn with_base_url(network: Arc<dyn NetworkAdapter>, base_url: &str) -> Self {
        Self {
            network,
            base_url: base_url.trim_end_matches('/').to_string(),
        }
    }
}

#[async_trait]
impl SearchProvider for MangabzSearchProvider {
    fn engine_name(&self) -> &str {
        "Mangabz"
    }

    async fn search(
        &self,
        keyword: &str,
        skip: i32,
        take: i32,
    ) -> anf_core::Result<SearchComicResult> {
        let page = if take > 0 { (skip / take).max(1) } else { 1 };
        let url = format!(
            "{}/search/?title={}&page={}",
            self.base_url,
            urlencoding::encode(keyword),
            page
        );
        let mut headers = std::collections::HashMap::new();
        headers.insert("User-Agent".into(), "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1".into());
        let settings = RequestSettings {
            address: url.clone(),
            referrer: Some(format!("{}/", self.base_url)),
            headers: Some(headers),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);

        let item_sel = Selector::parse("div.mh-item").unwrap();
        let items: Vec<_> = doc
            .select(&item_sel)
            .skip(skip as usize)
            .take(take as usize)
            .collect();
        let total = items.len();
        let mut snapshots = Vec::new();

        for item in items {
            let a_sel = Selector::parse("div.mh-item-detali h2.title a").unwrap();
            let (title, href) = if let Some(a) = item.select(&a_sel).next() {
                (
                    a.text().collect::<String>().trim().to_string(),
                    a.value().attr("href").unwrap_or("").to_string(),
                )
            } else {
                continue;
            };

            let cover_sel = Selector::parse("p").unwrap();
            let cover = item
                .select(&cover_sel)
                .next()
                .and_then(|n| n.value().attr("style"))
                .and_then(|s| {
                    let l = s.find('(')?;
                    let r = s.rfind(')')?;
                    Some(s[l + 1..r].to_string())
                })
                .unwrap_or_default();

            let auth_sel = Selector::parse("p.author span a").unwrap();
            let auth = item
                .select(&auth_sel)
                .next()
                .map(|n| n.text().collect::<String>().trim().to_string())
                .unwrap_or_default();

            snapshots.push(ComicSnapshot {
                name: title,
                author: auth,
                image_uri: cover,
                target_url: url.clone(),
                sources: vec![ComicSource {
                    target_url: format!("{}{href}", self.base_url),
                    name: "Mangabz".into(),
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
