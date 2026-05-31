use anf_core::{
    ComicSnapshot, ComicSource, NetworkAdapter, RequestSettings, SearchComicResult, SearchProvider,
};
use async_trait::async_trait;
use scraper::{Html, Selector};
use std::sync::Arc;

pub struct Dm5SearchProvider {
    network: Arc<dyn NetworkAdapter>,
    base_url: String,
}
impl Dm5SearchProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self {
            network,
            base_url: "http://www.dm5.com".into(),
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
impl SearchProvider for Dm5SearchProvider {
    fn engine_name(&self) -> &str {
        "Dm5"
    }
    async fn search(
        &self,
        keyword: &str,
        skip: i32,
        take: i32,
    ) -> anf_core::Result<SearchComicResult> {
        let url = format!(
            "{}/search?title={}&language=1",
            self.base_url,
            urlencoding::encode(keyword)
        );
        let host = url::Url::parse(&self.base_url)
            .ok()
            .and_then(|u| u.host_str().map(|h| h.to_string()))
            .unwrap_or_default();
        let settings = RequestSettings {
            address: url.clone(),
            host: Some(host.clone()),
            referrer: Some(format!("{}/", self.base_url)),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);
        let box_sel = Selector::parse("div.box-body ul li div.mh-item").unwrap();
        let items: Vec<_> = doc
            .select(&box_sel)
            .skip(skip as usize)
            .take(take as usize)
            .collect();
        let total = items.len();
        let mut snapshots = Vec::new();
        for item in items {
            let detail_sel = Selector::parse("div.mh-item-detali h2.title a").unwrap();
            let (title, href) = if let Some(a) = item.select(&detail_sel).next() {
                (
                    a.text().collect::<String>().trim().to_string(),
                    a.value().attr("href").unwrap_or("").to_string(),
                )
            } else {
                continue;
            };
            let auth_sel = Selector::parse("p.author span a").unwrap();
            let auth = item
                .select(&auth_sel)
                .next()
                .map(|n| n.text().collect::<String>())
                .unwrap_or_default();
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
            snapshots.push(ComicSnapshot {
                name: title,
                author: auth,
                image_uri: cover,
                target_url: url.clone(),
                sources: vec![ComicSource {
                    target_url: format!("{}{href}", self.base_url),
                    name: "Dm5".into(),
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
