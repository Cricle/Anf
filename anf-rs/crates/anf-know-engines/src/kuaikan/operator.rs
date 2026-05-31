use anf_core::{
    ComicChapter, ComicEntity, ComicInfo, ComicPage, ComicSourceProvider, JsonVisitor,
    NetworkAdapter, RequestSettings,
};
use async_trait::async_trait;
use bytes::Bytes;
use regex::Regex;
use scraper::{Html, Selector};
use std::sync::Arc;

use crate::js_eval;

pub struct KuaikanProvider {
    network: Arc<dyn NetworkAdapter>,
}
impl KuaikanProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl ComicSourceProvider for KuaikanProvider {
    async fn get_chapters(&self, target_url: &str) -> anf_core::Result<ComicEntity> {
        let settings = RequestSettings {
            address: target_url.into(),
            referrer: Some("https://www.kuaikanmanhua.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);

        let title_sel = Selector::parse("div.TopicList div div.right h3").unwrap();
        let title = doc
            .select(&title_sel)
            .next()
            .map(|n| n.text().collect::<String>())
            .unwrap_or_else(|| target_url.rsplit('/').next().unwrap_or("").into());
        let desc_sel = Selector::parse("div.comicIntro div.details p").unwrap();
        let desc = doc
            .select(&desc_sel)
            .next()
            .map(|n| n.text().collect::<String>())
            .unwrap_or_default();
        let img_sel = Selector::parse("div.TopicItem div.left img.imgCover").unwrap();
        let img = doc
            .select(&img_sel)
            .next()
            .and_then(|n| n.value().attr("src"))
            .unwrap_or("")
            .to_string();

        let node_sel = Selector::parse("div.TopicItem.cls").unwrap();
        let link_sel = Selector::parse("div.cover a").unwrap();
        let mut chapters = Vec::new();
        for node in doc.select(&node_sel) {
            if let Some(a) = node.select(&link_sel).next() {
                let href = a.value().attr("href").unwrap_or("");
                let alt_sel = Selector::parse("img").unwrap();
                let alt = a
                    .select(&alt_sel)
                    .nth(1)
                    .and_then(|n| n.value().attr("alt"))
                    .unwrap_or("")
                    .to_string();
                chapters.push(ComicChapter {
                    target_url: format!("https://www.kuaikanmanhua.com/{href}"),
                    title: alt,
                });
            }
        }
        chapters.reverse();
        Ok(ComicEntity {
            info: ComicInfo {
                comic_url: target_url.into(),
                name: title,
                descript: desc,
                image_url: img,
            },
            chapters,
        })
    }

    async fn get_pages(&self, target_url: &str) -> anf_core::Result<Vec<ComicPage>> {
        let settings = RequestSettings {
            address: target_url.into(),
            referrer: Some("https://www.kuaikanmanhua.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;

        // Kuaikan embeds __NUXT__ data in a script tag
        let re = Regex::new(r#"<script>window\.__NUXT__=(.*?);</script>"#).unwrap();
        if let Some(caps) = re.captures(&str_) {
            let js_code = caps.get(1).map(|m| m.as_str()).unwrap_or("");
            // Evaluate the __NUXT__ expression and extract comic_images
            let wrapped = format!(
                "var window = {{}}; var __nuxt_result__ = ({}); JSON.stringify(__nuxt_result__)",
                js_code
            );
            match js_eval::eval(&wrapped) {
                Ok(json_str) => {
                    if let Ok(jv_result) = JsonVisitor::from_str(&json_str) {
                        // Navigate: data[0].res.data.comic_info.comic_images
                        let data = jv_result.get("data");
                        if let Some(first) = data.to_vec().first() {
                            let comic_info = first.get("res").get("data").get("comic_info");
                            let images = comic_info.get("comic_images").to_vec();
                            if !images.is_empty() {
                                return Ok(images
                                    .iter()
                                    .enumerate()
                                    .map(|(i, item)| ComicPage {
                                        name: (i + 1).to_string(),
                                        target_url: item.get("url").to_string(),
                                    })
                                    .collect());
                            }
                        }
                    }
                }
                Err(e) => tracing::warn!(error = %e, "kuaikan js eval failed"),
            }
        }
        Ok(Vec::new())
    }

    async fn get_image_stream(&self, target_url: &str) -> anf_core::Result<Bytes> {
        let settings = RequestSettings {
            address: target_url.into(),
            referrer: Some("https://www.kuaikanmanhua.com/".into()),
            ..Default::default()
        };
        self.network.get_stream(&settings).await
    }
}
