use anf_core::{ComicSnapshot, ComicSource, NetworkAdapter, ProposalProvider, RequestSettings};
use async_trait::async_trait;
use scraper::{Html, Selector};
use std::sync::Arc;

pub struct TencentProposalProvider {
    network: Arc<dyn NetworkAdapter>,
}
impl TencentProposalProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl ProposalProvider for TencentProposalProvider {
    fn engine_name(&self) -> &str {
        "Tencent"
    }

    async fn get_proposal(&self, take: i32) -> anf_core::Result<Vec<ComicSnapshot>> {
        let settings = RequestSettings {
            address: "https://ac.qq.com/".into(),
            host: Some("ac.qq.com".into()),
            referrer: Some("https://ac.qq.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);

        let item_sel = Selector::parse("div.ret-works-cover a").unwrap();
        let mut snapshots = Vec::new();
        for a in doc.select(&item_sel) {
            if snapshots.len() >= take as usize {
                break;
            }
            let href = a.value().attr("href").unwrap_or("");
            let img_sel = Selector::parse("img").unwrap();
            let img = a
                .select(&img_sel)
                .next()
                .and_then(|n| n.value().attr("src"))
                .unwrap_or("")
                .to_string();
            let title = a
                .select(&img_sel)
                .next()
                .and_then(|n| n.value().attr("alt"))
                .unwrap_or("")
                .to_string();
            if title.is_empty() {
                continue;
            }
            snapshots.push(ComicSnapshot {
                name: title,
                author: String::new(),
                image_uri: img,
                target_url: format!("https://ac.qq.com{href}"),
                sources: vec![ComicSource {
                    target_url: format!("https://ac.qq.com{href}"),
                    name: "Tencent".into(),
                }],
                descript: String::new(),
            });
        }
        Ok(snapshots)
    }
}
