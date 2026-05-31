use anf_core::{ComicSnapshot, ComicSource, NetworkAdapter, ProposalProvider, RequestSettings};
use async_trait::async_trait;
use scraper::{Html, Selector};
use std::sync::Arc;

pub struct Dm5ProposalProvider {
    network: Arc<dyn NetworkAdapter>,
    base_url: String,
}
impl Dm5ProposalProvider {
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
impl ProposalProvider for Dm5ProposalProvider {
    fn engine_name(&self) -> &str {
        "Dm5"
    }
    async fn get_proposal(&self, take: i32) -> anf_core::Result<Vec<ComicSnapshot>> {
        let host = url::Url::parse(&self.base_url)
            .ok()
            .and_then(|u| u.host_str().map(|h| h.to_string()))
            .unwrap_or_default();
        let settings = RequestSettings {
            address: format!("{}/", self.base_url),
            host: Some(host),
            referrer: Some(format!("{}/", self.base_url)),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);
        let mut snapshots = Vec::new();
        for i in 1..7 {
            if snapshots.len() >= take as usize {
                break;
            }
            let sel = Selector::parse(&format!(
                "div#index-update-{} div ul li div div.mh-tip-wrap div",
                i
            ))
            .unwrap();
            for root in doc.select(&sel) {
                if snapshots.len() >= take as usize {
                    break;
                }
                let a_sel = Selector::parse("a").unwrap();
                let href = root
                    .select(&a_sel)
                    .next()
                    .and_then(|n| n.value().attr("href"))
                    .unwrap_or("");
                let title_sel = Selector::parse("div.mh-item-tip-detali h2 a").unwrap();
                let title = root
                    .select(&title_sel)
                    .next()
                    .and_then(|n| n.value().attr("title"))
                    .unwrap_or("")
                    .to_string();
                let auth_sel = Selector::parse("div.mh-item-tip-detali p.author span a").unwrap();
                let auth = root
                    .select(&auth_sel)
                    .next()
                    .map(|n| n.text().collect::<String>().trim().to_string())
                    .unwrap_or_default();
                snapshots.push(ComicSnapshot {
                    name: title,
                    author: auth,
                    target_url: format!("{}/", self.base_url),
                    image_uri: String::new(),
                    sources: vec![ComicSource {
                        target_url: format!("{}{href}", self.base_url),
                        name: "Dm5".into(),
                    }],
                    descript: String::new(),
                });
            }
        }
        Ok(snapshots)
    }
}
