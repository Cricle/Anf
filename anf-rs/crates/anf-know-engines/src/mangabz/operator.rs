use anf_core::{
    ComicChapter, ComicEntity, ComicInfo, ComicPage, ComicSourceProvider, NetworkAdapter,
    RequestSettings,
};
use async_trait::async_trait;
use bytes::Bytes;
use regex::Regex;
use scraper::{Html, Selector};
use std::sync::Arc;

use crate::js_eval;

pub struct MangabzProvider {
    network: Arc<dyn NetworkAdapter>,
    base_url: String,
}
impl MangabzProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self {
            network,
            base_url: "http://www.mangabz.com".into(),
        }
    }
    pub fn with_base_url(network: Arc<dyn NetworkAdapter>, base_url: &str) -> Self {
        Self {
            network,
            base_url: base_url.into(),
        }
    }
    fn base_referrer(&self) -> String {
        format!("{}/", self.base_url)
    }
    fn user_agent(&self) -> &'static str {
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1"
    }
}

#[async_trait]
impl ComicSourceProvider for MangabzProvider {
    async fn get_chapters(&self, target_url: &str) -> anf_core::Result<ComicEntity> {
        let mut headers = std::collections::HashMap::new();
        headers.insert("User-Agent".into(), self.user_agent().into());
        let settings = RequestSettings {
            address: target_url.into(),
            host: Some(anf_core::url_helper::fast_get_host(target_url).into()),
            referrer: Some(self.base_referrer()),
            headers: Some(headers),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);

        let img_sel = Selector::parse("img.detail-info-cover").unwrap();
        let img = doc
            .select(&img_sel)
            .next()
            .and_then(|n| n.value().attr("src"))
            .unwrap_or("")
            .to_string();
        let title_sel = Selector::parse("p.detail-info-title").unwrap();
        let title = doc
            .select(&title_sel)
            .next()
            .map(|n| n.text().collect::<String>())
            .unwrap_or_default();
        let desc_sel = Selector::parse("div.detail-info-content").unwrap();
        let desc = doc
            .select(&desc_sel)
            .next()
            .map(|n| n.text().collect::<String>())
            .unwrap_or_default();

        let link_sel = Selector::parse("div#chapterlistload a").unwrap();
        let mut chapters: Vec<ComicChapter> = doc
            .select(&link_sel)
            .map(|n| ComicChapter {
                title: n.text().collect::<String>().replace("  ", ""),
                target_url: format!("{}{}", self.base_url, n.value().attr("href").unwrap_or("")),
            })
            .collect();
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
        let mut headers = std::collections::HashMap::new();
        headers.insert("User-Agent".into(), self.user_agent().into());
        let settings = RequestSettings {
            address: target_url.into(),
            referrer: Some(self.base_referrer()),
            headers: Some(headers),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;

        let cid_re = Regex::new(r"var MANGABZ_CID=(\d+);").unwrap();
        let _dt_re = Regex::new(r#"var MANGABZ_VIEWSIGN_DT="([^"]*)";"#).unwrap();
        let mid_re = Regex::new(r"var MANGABZ_MID=(\d+);").unwrap();
        let sign_re = Regex::new(r#"var MANGABZ_VIEWSIGN="([^"]*)";"#).unwrap();
        let count_re = Regex::new(r"var MANGABZ_IMAGE_COUNT=(\d+);").unwrap();

        let cid = cid_re
            .captures(&str_)
            .and_then(|c| c.get(1))
            .map(|m| m.as_str())
            .unwrap_or("0");
        let mid = mid_re
            .captures(&str_)
            .and_then(|c| c.get(1))
            .map(|m| m.as_str())
            .unwrap_or("0");
        let sign = sign_re
            .captures(&str_)
            .and_then(|c| c.get(1))
            .map(|m| m.as_str())
            .unwrap_or("");
        let img_count: i32 = count_re
            .captures(&str_)
            .and_then(|c| c.get(1))
            .map(|m| m.as_str().parse().unwrap_or(0))
            .unwrap_or(0);

        if img_count == 0 {
            return Ok(Vec::new());
        }

        let ref_addr = target_url.rsplit('/').next().unwrap_or("");
        let mut all_pages = Vec::new();
        let mut seen = std::collections::HashSet::new();

        for page_idx in 0..img_count {
            let ajax_url = format!(
                "{}/{}/chapterimage.ashx?cid={}&page={}&key=&_cid={}&_mid={}&_dt={}&_sign={}",
                self.base_url,
                ref_addr,
                cid,
                page_idx + 1,
                cid,
                mid,
                chrono::Utc::now().format("%Y-%m-%d+%H%%3A%M%%3A%S"),
                sign
            );
            let ajax_settings = RequestSettings {
                address: ajax_url,
                referrer: Some(self.base_referrer()),
                headers: Some({
                    let mut h = std::collections::HashMap::new();
                    h.insert("User-Agent".into(), self.user_agent().into());
                    h
                }),
                ..Default::default()
            };
            match self.network.get_string(&ajax_settings).await {
                Ok(js_code) => {
                    if js_code.trim().is_empty() {
                        continue;
                    }
                    match js_eval::eval_to_vec(&js_code) {
                        Ok(urls) => {
                            for url in urls {
                                let url = url.trim().to_string();
                                if !url.is_empty() && seen.insert(url.clone()) {
                                    all_pages.push(ComicPage {
                                        name: (all_pages.len() + 1).to_string(),
                                        target_url: url,
                                    });
                                }
                            }
                        }
                        Err(e) => {
                            tracing::warn!(page = page_idx, error = %e, "mangabz js eval failed")
                        }
                    }
                }
                Err(e) => tracing::warn!(page = page_idx, error = %e, "mangabz ajax failed"),
            }
        }
        Ok(all_pages)
    }

    async fn get_image_stream(&self, target_url: &str) -> anf_core::Result<Bytes> {
        let mut headers = std::collections::HashMap::new();
        headers.insert("User-Agent".into(), self.user_agent().into());
        let settings = RequestSettings {
            address: target_url.into(),
            referrer: Some(self.base_referrer()),
            headers: Some(headers),
            ..Default::default()
        };
        self.network.get_stream(&settings).await
    }
}
