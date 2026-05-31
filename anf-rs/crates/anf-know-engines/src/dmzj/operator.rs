use anf_core::{
    ComicChapter, ComicEntity, ComicInfo, ComicPage, ComicSourceProvider, NetworkAdapter,
    RequestSettings,
};
use async_trait::async_trait;
use bytes::Bytes;
use regex::Regex;
use scraper::{Html, Selector};
use std::sync::Arc;
use urlencoding;

use crate::js_eval;

pub struct DmzjProvider {
    network: Arc<dyn NetworkAdapter>,
}

impl DmzjProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }

    fn ensure_url(url: &str) -> String {
        if !url.starts_with("http://") && !url.starts_with("https://") {
            format!("http://{url}")
        } else {
            url.to_string()
        }
    }
}

#[async_trait]
impl ComicSourceProvider for DmzjProvider {
    async fn get_chapters(&self, target_url: &str) -> anf_core::Result<ComicEntity> {
        let url = Self::ensure_url(target_url);
        let settings = RequestSettings {
            address: url,
            referrer: Some("https://www.dmzj.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);

        let title_sel = Selector::parse("div.comic_deCon h1 a, span.anim_title_text a h1").unwrap();
        let title = doc
            .select(&title_sel)
            .next()
            .map(|n| n.text().collect::<String>().trim().to_string())
            .unwrap_or_else(|| target_url.rsplit('/').next().unwrap_or("").to_string());

        let desc_sel = Selector::parse("div.comic_deCon_d").unwrap();
        let desc = doc
            .select(&desc_sel)
            .next()
            .map(|n| n.text().collect::<String>())
            .unwrap_or_default();

        let img_sel = Selector::parse("div.comic_i_img a img").unwrap();
        let img = doc
            .select(&img_sel)
            .next()
            .and_then(|n| n.value().attr("src"))
            .unwrap_or("")
            .to_string();

        let link_sel =
            Selector::parse("div.cartoon_online_border ul li a, ul.list_con_li li a").unwrap();
        let mut chapters = Vec::new();
        for node in doc.select(&link_sel) {
            let name = node.text().collect::<String>();
            let href = node.value().attr("href").unwrap_or("").to_string();
            let href = if !href.starts_with("http") {
                format!("https://manhua.dmzj.com{href}")
            } else {
                href
            };
            chapters.push(ComicChapter {
                target_url: href,
                title: urlencoding::decode(&name).unwrap_or_default().into_owned(),
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
            referrer: Some("https://www.dmzj.com/".into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;

        // Dmzj embeds page URLs in an eval() block
        let re = Regex::new(r"eval\((.+?\{.*?\}\))\)").unwrap();
        if let Some(caps) = re.captures(&str_) {
            let js_block = caps.get(0).map(|m| m.as_str()).unwrap_or("");
            // The eval extracts a variable `pages` containing URL array
            // Wrap it to extract the pages variable
            let wrapped = format!("{}; JSON.stringify(pages)", js_block);
            match js_eval::eval(&wrapped) {
                Ok(json_str) => {
                    if let Ok(urls) = serde_json::from_str::<Vec<String>>(&json_str) {
                        let pages: Vec<ComicPage> = urls
                            .iter()
                            
                            .map(|url| {
                                let url = url.trim().to_string();
                                let name = url.rsplit('/').next().unwrap_or(&url).to_string();
                                ComicPage {
                                    name: urlencoding::decode(&name)
                                        .unwrap_or_default()
                                        .into_owned(),
                                    target_url: if url.starts_with("http") {
                                        url
                                    } else {
                                        format!("https://images.dmzj.com/{url}")
                                    },
                                }
                            })
                            .collect();
                        if !pages.is_empty() {
                            return Ok(pages);
                        }
                    }
                }
                Err(e) => tracing::warn!(error = %e, "dmzj js eval failed, falling back to regex"),
            }
        }

        // Fallback: extract image URLs directly from page source
        let url_re = Regex::new(r#"https?://images\.dmzj[0-9]*\.com/[^"'\s]+"#).unwrap();
        let pages: Vec<ComicPage> = url_re
            .find_iter(&str_)
            .enumerate()
            .map(|(i, m)| ComicPage {
                name: (i + 1).to_string(),
                target_url: m.as_str().to_string(),
            })
            .collect();
        if !pages.is_empty() {
            return Ok(pages);
        }

        // Also try the old-style page_url variable
        let page_url_re = Regex::new(r#"page_url\s*[:=]\s*"([^"]+)""#).unwrap();
        if let Some(caps) = page_url_re.captures(&str_) {
            if let Some(url_match) = caps.get(1) {
                let urls: Vec<&str> = url_match
                    .as_str()
                    .split("\\n")
                    .filter(|s| !s.trim().is_empty())
                    .collect();
                let pages: Vec<ComicPage> = urls
                    .iter()
                    .enumerate()
                    .map(|(i, url)| ComicPage {
                        name: (i + 1).to_string(),
                        target_url: format!("https://images.dmzj.com/{}", url.trim()),
                    })
                    .collect();
                return Ok(pages);
            }
        }

        Ok(Vec::new())
    }

    async fn get_image_stream(&self, target_url: &str) -> anf_core::Result<Bytes> {
        let settings = RequestSettings {
            address: target_url.to_string(),
            referrer: Some("https://www.dmzj.com/".into()),
            host: Some("images.dmzj1.com".into()),
            ..Default::default()
        };
        self.network.get_stream(&settings).await
    }
}
