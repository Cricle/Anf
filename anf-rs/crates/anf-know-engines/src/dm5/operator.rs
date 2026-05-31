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

pub struct Dm5Provider {
    network: Arc<dyn NetworkAdapter>,
    base_url: String,
}

impl Dm5Provider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self {
            network,
            base_url: "http://www.dm5.com".into(),
        }
    }
    pub fn with_base_url(network: Arc<dyn NetworkAdapter>, base_url: &str) -> Self {
        Self {
            network,
            base_url: base_url.into(),
        }
    }
    fn base_host(&self) -> &str {
        if self.base_url.contains("1kkk") {
            "www.1kkk.com"
        } else {
            "www.dm5.com"
        }
    }
}

#[async_trait]
impl ComicSourceProvider for Dm5Provider {
    async fn get_chapters(&self, target_url: &str) -> anf_core::Result<ComicEntity> {
        let settings = RequestSettings {
            address: target_url.to_string(),
            host: Some(self.base_host().to_string()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);

        let title_sel = Selector::parse("div.banner_detail_form div.info p.title").unwrap();
        let title = doc
            .select(&title_sel)
            .next()
            .map(|n| n.text().collect::<String>().trim().to_string())
            .unwrap_or_else(|| target_url.rsplit('/').next().unwrap_or("").to_string());

        let desc_sel = Selector::parse("div.banner_detail_form div.info p.content").unwrap();
        let desc = doc
            .select(&desc_sel)
            .next()
            .map(|n| n.text().collect::<String>())
            .unwrap_or_default();

        let img_sel = Selector::parse("div.banner_detail_form div.cover img").unwrap();
        let img = doc
            .select(&img_sel)
            .next()
            .and_then(|n| n.value().attr("src"))
            .unwrap_or("")
            .to_string();

        let link_sel = Selector::parse("ul#detail-list-select-1 li a").unwrap();
        let mut chapters = Vec::new();
        for node in doc.select(&link_sel) {
            let href = node.value().attr("href").unwrap_or("");
            let text = node.text().collect::<String>().trim().to_string();
            chapters.push(ComicChapter {
                target_url: format!("{}{}", self.base_url, href),
                title: text,
            });
        }
        chapters.reverse();
        Ok(ComicEntity {
            info: ComicInfo {
                comic_url: target_url.to_string(),
                name: title,
                descript: desc,
                image_url: img,
            },
            chapters,
        })
    }

    async fn get_pages(&self, target_url: &str) -> anf_core::Result<Vec<ComicPage>> {
        let settings = RequestSettings {
            address: target_url.to_string(),
            host: Some(self.base_host().to_string()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;

        let cid_re = Regex::new(r"var DM5_CID=(\d+);").unwrap();
        let dt_re = Regex::new(r#"var DM5_VIEWSIGN_DT="([^"]*)";"#).unwrap();
        let mid_re = Regex::new(r"var DM5_MID=(\d+);").unwrap();
        let sign_re = Regex::new(r#"var DM5_VIEWSIGN="([^"]*)";"#).unwrap();
        let count_re = Regex::new(r"var DM5_IMAGE_COUNT=(\d+);").unwrap();

        let cid = cid_re
            .captures(&str_)
            .and_then(|c| c.get(1))
            .map(|m| m.as_str())
            .unwrap_or("0");
        let dt = dt_re
            .captures(&str_)
            .and_then(|c| c.get(1))
            .map(|m| m.as_str())
            .unwrap_or("");
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
                "{}/{}/chapterfun.ashx?cid={}&page={}&key=&language=1&gtk=6&_cid={}&_mid={}&_dt={}&_sign={}",
                self.base_url, ref_addr, cid, page_idx + 1, cid, mid, dt, sign
            );
            let ajax_settings = RequestSettings {
                address: ajax_url,
                host: Some(self.base_host().to_string()),
                referrer: Some(target_url.to_string()),
                ..Default::default()
            };
            match self.network.get_string(&ajax_settings).await {
                Ok(js_code) => {
                    if js_code.trim().is_empty() {
                        continue;
                    }
                    // The response is a JS expression that evaluates to an array of URLs
                    match js_eval::eval_to_vec(&js_code) {
                        Ok(urls) => {
                            for url in urls {
                                let url = url.trim().to_string();
                                if !url.is_empty() && seen.insert(url.clone()) {
                                    all_pages.push(ComicPage {
                                        name: (all_pages.len() + 1).to_string(),
                                        target_url: if url.starts_with("http") {
                                            url
                                        } else {
                                            format!("https://images.dmzj.com/{}", url)
                                        },
                                    });
                                }
                            }
                        }
                        Err(e) => {
                            tracing::warn!(page = page_idx, error = %e, "dm5 js eval failed");
                        }
                    }
                }
                Err(e) => {
                    tracing::warn!(page = page_idx, error = %e, "dm5 ajax request failed");
                }
            }
        }
        Ok(all_pages)
    }

    async fn get_image_stream(&self, target_url: &str) -> anf_core::Result<Bytes> {
        let settings = RequestSettings {
            address: target_url.to_string(),
            host: Some(self.base_host().to_string()),
            ..Default::default()
        };
        self.network.get_stream(&settings).await
    }
}
