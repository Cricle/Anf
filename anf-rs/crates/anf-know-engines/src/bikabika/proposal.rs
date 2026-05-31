use anf_core::{ComicSnapshot, ComicSource, NetworkAdapter, ProposalProvider, RequestSettings};
use async_trait::async_trait;
use scraper::{Html, Selector};
use std::sync::Arc;

pub struct BikabikaProposalProvider {
    network: Arc<dyn NetworkAdapter>,
}

impl BikabikaProposalProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl ProposalProvider for BikabikaProposalProvider {
    fn engine_name(&self) -> &str {
        "Bikabika"
    }

    async fn get_proposal(&self, take: i32) -> anf_core::Result<Vec<ComicSnapshot>> {
        let mut headers = std::collections::HashMap::new();
        headers.insert("User-Agent".into(), "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1".into());
        let settings = RequestSettings {
            address: "http://www.bikabika.com/".into(),
            referrer: Some("http://www.bikabika.com/".into()),
            headers: Some(headers),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);

        let mut snapshots = Vec::new();

        // Get popular comics from homepage
        let item_sel =
            Selector::parse("div.hot-item, div.recommend-item, div.manga-item, div.comic-item")
                .unwrap();
        for item in doc.select(&item_sel) {
            if snapshots.len() >= take as usize {
                break;
            }

            let a_sel = Selector::parse("a").unwrap();
            let (title, href) = if let Some(a) = item.select(&a_sel).next() {
                let title = a
                    .value()
                    .attr("title")
                    .map(|s| s.to_string())
                    .unwrap_or_else(|| a.text().collect::<String>().trim().to_string());
                (title, a.value().attr("href").unwrap_or("").to_string())
            } else {
                continue;
            };

            if title.is_empty() || href.is_empty() {
                continue;
            }

            let img_sel = Selector::parse("img").unwrap();
            let cover = item
                .select(&img_sel)
                .next()
                .and_then(|n| n.value().attr("src"))
                .unwrap_or("")
                .to_string();

            snapshots.push(ComicSnapshot {
                name: title,
                author: String::new(),
                image_uri: cover,
                target_url: "http://www.bikabika.com/".into(),
                sources: vec![ComicSource {
                    target_url: format!("http://www.bikabika.com{href}"),
                    name: "Bikabika".into(),
                }],
                descript: String::new(),
            });
        }

        Ok(snapshots)
    }
}
