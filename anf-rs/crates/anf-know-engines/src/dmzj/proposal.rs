use anf_core::{ComicSnapshot, ComicSource, NetworkAdapter, ProposalProvider, RequestSettings};
use async_trait::async_trait;
use scraper::{Html, Selector};
use std::sync::Arc;

pub struct DmzjProposalProvider {
    network: Arc<dyn NetworkAdapter>,
}

impl DmzjProposalProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl ProposalProvider for DmzjProposalProvider {
    fn engine_name(&self) -> &str {
        "DMZJ"
    }

    async fn get_proposal(&self, take: i32) -> anf_core::Result<Vec<ComicSnapshot>> {
        let settings = RequestSettings {
            address: "https://www.dmzj.com/".into(),
            referrer: Some("https://www.dmzj.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);

        let mut snapshots = Vec::new();

        // Get hot comics from the homepage
        let hot_sel =
            Selector::parse("div.hot_mh ul li, div.rank-list div.rank-item, div.recommend-item")
                .unwrap();
        for item in doc.select(&hot_sel) {
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

            let href = if !href.starts_with("http") {
                format!("https://manhua.dmzj.com{href}")
            } else {
                href
            };

            snapshots.push(ComicSnapshot {
                name: title,
                author: String::new(),
                image_uri: cover,
                target_url: href.clone(),
                sources: vec![ComicSource {
                    target_url: href,
                    name: "DMZJ".into(),
                }],
                descript: String::new(),
            });
        }

        Ok(snapshots)
    }
}
