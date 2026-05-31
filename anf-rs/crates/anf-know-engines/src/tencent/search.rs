use anf_core::{
    ComicSnapshot, ComicSource, NetworkAdapter, RequestSettings, SearchComicResult, SearchProvider,
};
use async_trait::async_trait;
use scraper::{Html, Selector};
use std::sync::Arc;

pub struct TencentSearchProvider {
    network: Arc<dyn NetworkAdapter>,
}
impl TencentSearchProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl SearchProvider for TencentSearchProvider {
    fn engine_name(&self) -> &str {
        "Tencent"
    }

    async fn search(
        &self,
        keyword: &str,
        skip: i32,
        take: i32,
    ) -> anf_core::Result<SearchComicResult> {
        let page = if take > 0 { (skip / take).max(1) } else { 1 };
        let url = format!(
            "https://ac.qq.com/Comic/search/page/{}?keyword={}",
            page,
            urlencoding::encode(keyword)
        );
        let settings = RequestSettings {
            address: url.clone(),
            host: Some("ac.qq.com".into()),
            referrer: Some("https://ac.qq.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);

        let item_sel = Selector::parse("li.ret-search-item").unwrap();
        let items: Vec<_> = doc
            .select(&item_sel)
            .skip(skip as usize)
            .take(take as usize)
            .collect();
        let total = items.len();
        let mut snapshots = Vec::new();

        for item in items {
            let img_sel = Selector::parse("img").unwrap();
            let img = item
                .select(&img_sel)
                .next()
                .and_then(|n| n.value().attr("src"))
                .unwrap_or("")
                .to_string();

            let title_sel = Selector::parse("h3.ret-works-title a").unwrap();
            let (title, href) = if let Some(a) = item.select(&title_sel).next() {
                (
                    a.text().collect::<String>().trim().to_string(),
                    a.value().attr("href").unwrap_or("").to_string(),
                )
            } else {
                continue;
            };

            let auth_sel = Selector::parse("p.ret-works-author a").unwrap();
            let auth = item
                .select(&auth_sel)
                .next()
                .map(|n| n.text().collect::<String>().trim().to_string())
                .unwrap_or_default();

            let desc_sel = Selector::parse("p.ret-works-decs").unwrap();
            let desc = item
                .select(&desc_sel)
                .next()
                .map(|n| n.text().collect::<String>().trim().to_string())
                .unwrap_or_default();

            snapshots.push(ComicSnapshot {
                name: title,
                author: auth,
                image_uri: img,
                descript: desc,
                target_url: url.clone(),
                sources: vec![ComicSource {
                    target_url: format!("https://ac.qq.com{href}"),
                    name: "Tencent".into(),
                }],
            });
        }
        Ok(SearchComicResult {
            support: true,
            snapshots,
            total: Some(total as i64),
        })
    }
}
