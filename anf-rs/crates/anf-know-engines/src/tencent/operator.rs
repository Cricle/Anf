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

pub struct TencentProvider {
    network: Arc<dyn NetworkAdapter>,
}
impl TencentProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

const JS_ADDR: &str = "https://ac.gtimg.com/media/js/ac.page.chapter.view_v2.6.0.js";

#[async_trait]
impl ComicSourceProvider for TencentProvider {
    async fn get_chapters(&self, target_url: &str) -> anf_core::Result<ComicEntity> {
        let settings = RequestSettings {
            address: target_url.into(),
            host: Some(anf_core::url_helper::fast_get_host(target_url).into()),
            referrer: Some(target_url.into()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);

        let img_sel = Selector::parse("div.works-cover a img").unwrap();
        let img = doc
            .select(&img_sel)
            .next()
            .and_then(|n| n.value().attr("src"))
            .unwrap_or("")
            .to_string();
        let name_sel = Selector::parse("h2.works-intro-title").unwrap();
        let name = doc
            .select(&name_sel)
            .next()
            .map(|n| n.text().collect::<String>())
            .unwrap_or_default();
        let desc_sel = Selector::parse("p.works-intro-short").unwrap();
        let desc = doc
            .select(&desc_sel)
            .next()
            .map(|n| n.text().collect::<String>().trim().to_string())
            .unwrap_or_default();

        let link_sel = Selector::parse("ol li p span a").unwrap();
        let chapters: Vec<ComicChapter> = doc
            .select(&link_sel)
            .map(|n| ComicChapter {
                target_url: format!("https://ac.qq.com{}", n.value().attr("href").unwrap_or("")),
                title: n.text().collect::<String>().trim().to_string(),
            })
            .collect();

        Ok(ComicEntity {
            info: ComicInfo {
                comic_url: target_url.into(),
                name,
                descript: desc,
                image_url: img,
            },
            chapters,
        })
    }

    async fn get_pages(&self, target_url: &str) -> anf_core::Result<Vec<ComicPage>> {
        let mut img_headers = std::collections::HashMap::new();
        img_headers.insert("User-Agent".into(), "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1".into());

        let settings = RequestSettings {
            address: target_url.into(),
            headers: Some(img_headers.clone()),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;

        // Extract DATA variable
        let data_re = Regex::new(r"var\s+DATA\s*=\s*'([^']+)'").unwrap();
        let data_match = match data_re.captures(&str_) {
            Some(c) => c.get(1).map(|m| m.as_str()).unwrap_or(""),
            None => return Ok(Vec::new()),
        };

        // Fetch the JS eval file
        let js_settings = RequestSettings {
            address: JS_ADDR.into(),
            ..Default::default()
        };
        let js_code = self.network.get_string(&js_settings).await?;

        // Extract the eval block
        let eval_start = js_code.find("eval(function(p,a,c,k,e,r)");
        let eval_end = js_code.find("}();");
        if let (Some(start), Some(end)) = (eval_start, eval_end) {
            let eval_block = &js_code[start..end + 4];

            // Extract window["n..."] variable assignment
            let notic_re = Regex::new(r#"window\["n[^;]*;"#).unwrap();
            let notic = notic_re.find(&str_).map(|m| m.as_str()).unwrap_or("");

            // Combine and eval
            let var_js = "var window={};var W=window;var _v={};";
            let combined = format!(
                "{}var DATA='{}';window.DATA=DATA;{};{};JSON.stringify(_v);",
                var_js, data_match, notic, eval_block
            );

            match js_eval::eval(&combined) {
                Ok(json_str) => {
                    if let Ok(jv) = JsonVisitor::from_str(&json_str) {
                        let pics = jv.get("picture").to_vec();
                        let pages: Vec<ComicPage> = pics
                            .iter()
                            
                            .map(|item| ComicPage {
                                name: item.get("pid").to_string(),
                                target_url: item.get("url").to_string(),
                            })
                            .collect();
                        return Ok(pages);
                    }
                }
                Err(e) => tracing::warn!(error = %e, "tencent js eval failed"),
            }
        }
        Ok(Vec::new())
    }

    async fn get_image_stream(&self, target_url: &str) -> anf_core::Result<Bytes> {
        let mut headers = std::collections::HashMap::new();
        headers.insert("authority".into(), "manhua.acimg.cn".into());
        headers.insert(
            "accept".into(),
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8".into(),
        );
        headers.insert("User-Agent".into(), "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1".into());
        let settings = RequestSettings {
            address: target_url.into(),
            headers: Some(headers),
            ..Default::default()
        };
        self.network.get_stream(&settings).await
    }
}
