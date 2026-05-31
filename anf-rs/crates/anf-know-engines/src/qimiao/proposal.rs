use anf_core::{
    ComicSnapshot, ComicSource, JsonVisitor, NetworkAdapter, ProposalProvider, RequestSettings,
};
use async_trait::async_trait;
use std::sync::Arc;

pub struct QimiaoProposalProvider {
    network: Arc<dyn NetworkAdapter>,
}

impl QimiaoProposalProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl ProposalProvider for QimiaoProposalProvider {
    fn engine_name(&self) -> &str {
        "Qimiao"
    }

    async fn get_proposal(&self, take: i32) -> anf_core::Result<Vec<ComicSnapshot>> {
        let settings = RequestSettings {
            address: "https://www.qimiaomh.com/api/recommend".into(),
            referrer: Some("https://www.qimiaomh.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let jv = JsonVisitor::from_str(&str_)?;
        let data = jv.get("data").to_vec();
        let mut snapshots = Vec::new();

        for (i, item) in data.iter().enumerate() {
            if i >= take as usize {
                break;
            }
            snapshots.push(ComicSnapshot {
                name: item.get("name").to_string(),
                author: item.get("author").to_string(),
                image_uri: item.get("cover").to_string(),
                descript: item.get("desc").to_string(),
                target_url: "https://www.qimiaomh.com/".into(),
                sources: vec![ComicSource {
                    target_url: format!(
                        "https://www.qimiaomh.com/comic/{}",
                        item.get("id")
                    ),
                    name: "Qimiao".into(),
                }],
            });
        }

        Ok(snapshots)
    }
}
