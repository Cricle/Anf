use anf_core::{
    ComicChapter, ComicEntity, ComicInfo, ComicPage, ComicSourceProvider, JsonVisitor,
    NetworkAdapter, RequestSettings,
};
use async_trait::async_trait;
use bytes::Bytes;
use scraper::{Html, Selector};
use std::sync::Arc;

pub struct QimiaoProvider {
    network: Arc<dyn NetworkAdapter>,
}
impl QimiaoProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl ComicSourceProvider for QimiaoProvider {
    async fn get_chapters(&self, target_url: &str) -> anf_core::Result<ComicEntity> {
        let settings = RequestSettings {
            address: target_url.into(),
            host: Some(anf_core::url_helper::fast_get_host(target_url).into()),
            referrer: Some("https://www.qimiaomh.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);
        let img_sel = Selector::parse("div.inner div.ctdbLeft a img").unwrap();
        let img = doc.select(&img_sel).next();
        let name = img
            .as_ref()
            .and_then(|n| n.value().attr("alt"))
            .unwrap_or("")
            .to_string();
        let img_url = img
            .and_then(|n| n.value().attr("src"))
            .unwrap_or("")
            .to_string();
        let desc_sel = Selector::parse("p#worksDesc").unwrap();
        let desc = doc
            .select(&desc_sel)
            .next()
            .map(|n| n.text().collect::<String>())
            .unwrap_or_default();
        let link_sel = Selector::parse("div.comic-content-list ul li a").unwrap();
        let chapters: Vec<ComicChapter> = doc
            .select(&link_sel)
            .map(|n| ComicChapter {
                target_url: format!(
                    "https://www.qimiaomh.com{}",
                    n.value().attr("href").unwrap_or("")
                ),
                title: n.value().attr("title").unwrap_or("").to_string(),
            })
            .collect();
        Ok(ComicEntity {
            info: ComicInfo {
                comic_url: target_url.into(),
                name,
                descript: desc,
                image_url: img_url,
            },
            chapters,
        })
    }

    async fn get_pages(&self, target_url: &str) -> anf_core::Result<Vec<ComicPage>> {
        // Extract did/sid from URL path segments
        let path = target_url.trim_start_matches("https://www.qimiaomh.com/");
        let parts: Vec<&str> = path.trim_matches('/').split('/').collect();
        let seg = parts.iter().rev().nth(1).unwrap_or(&"");
        let idx = parts.last().unwrap_or(&"").replace(".html", "");
        let url =
            format!("https://www.qimiaomh.com/Action/Play/AjaxLoadImgUrl?did={seg}&sid={idx}");
        let settings = RequestSettings {
            address: url,
            referrer: Some("https://www.qimiaomh.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let jv = JsonVisitor::from_str(&str_)?;
        let imgs = jv.get("listImg").to_vec();
        Ok(imgs
            .iter()
            .enumerate()
            .map(|(i, item)| ComicPage {
                name: i.to_string(),
                target_url: item.to_string(),
            })
            .collect())
    }

    async fn get_image_stream(&self, target_url: &str) -> anf_core::Result<Bytes> {
        let settings = RequestSettings {
            address: target_url.into(),
            referrer: Some("https://www.qimiaomh.com/".into()),
            ..Default::default()
        };
        self.network.get_stream(&settings).await
    }
}
