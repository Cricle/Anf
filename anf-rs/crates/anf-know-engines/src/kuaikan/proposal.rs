use anf_core::{
    ComicSnapshot, ComicSource, JsonVisitor, NetworkAdapter, ProposalProvider, RequestSettings,
};
use async_trait::async_trait;
use std::sync::Arc;

pub struct KuaikanProposalProvider {
    network: Arc<dyn NetworkAdapter>,
}
impl KuaikanProposalProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl ProposalProvider for KuaikanProposalProvider {
    fn engine_name(&self) -> &str {
        "Kuaikan"
    }

    async fn get_proposal(&self, take: i32) -> anf_core::Result<Vec<ComicSnapshot>> {
        let url = format!(
            "https://www.kuaikanmanhua.com/v1/search/topic?q=&f=1&size={}",
            take
        );
        let settings = RequestSettings {
            address: url.clone(),
            referrer: Some("https://www.kuaikanmanhua.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let jv = JsonVisitor::from_str(&str_)?;
        let hit = jv.get("data").get("hit").to_vec();
        let snapshots: Vec<ComicSnapshot> = hit
            .iter()
            .take(take as usize)
            .map(|item| {
                let id = item.get("id").to_string();
                ComicSnapshot {
                    name: item.get("title").to_string(),
                    author: item.get("user").get("nickname").to_string(),
                    image_uri: item.get("vertical_image_url").to_string(),
                    descript: item.get("description").to_string(),
                    target_url: url.clone(),
                    sources: vec![ComicSource {
                        target_url: format!("https://www.kuaikanmanhua.com/web/topic/{id}"),
                        name: "Kuaikan".into(),
                    }],
                }
            })
            .collect();
        Ok(snapshots)
    }
}
