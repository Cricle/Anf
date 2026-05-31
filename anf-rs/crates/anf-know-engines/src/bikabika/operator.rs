use anf_core::{
    ComicChapter, ComicEntity, ComicInfo, ComicPage, ComicSourceProvider, NetworkAdapter,
    RequestSettings,
};
use async_trait::async_trait;
use base64::Engine;
use bytes::Bytes;
use scraper::{Html, Selector};
use std::sync::Arc;

pub struct BikabikaProvider {
    network: Arc<dyn NetworkAdapter>,
}
impl BikabikaProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }
}

#[async_trait]
impl ComicSourceProvider for BikabikaProvider {
    async fn get_chapters(&self, target_url: &str) -> anf_core::Result<ComicEntity> {
        let mut headers = std::collections::HashMap::new();
        headers.insert("User-Agent".into(), "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1".into());
        let settings = RequestSettings {
            address: target_url.into(),
            host: Some(anf_core::url_helper::fast_get_host(target_url).into()),
            referrer: Some("http://www.bikabika.com/".into()),
            headers: Some(headers),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let doc = Html::parse_document(&str_);
        let img_sel = Selector::parse("div.banner_detail_form div.cover img").unwrap();
        let img = doc
            .select(&img_sel)
            .next()
            .and_then(|n| n.value().attr("src"))
            .unwrap_or("")
            .to_string();
        let title_sel = Selector::parse("div.banner_detail_form div.info p.title").unwrap();
        let title = doc
            .select(&title_sel)
            .next()
            .map(|n| n.text().collect::<String>())
            .unwrap_or_default();
        let desc_sel = Selector::parse("p#a_closes").unwrap();
        let desc = doc
            .select(&desc_sel)
            .next()
            .map(|n| n.text().collect::<String>())
            .unwrap_or_default();
        let link_sel = Selector::parse("div#chapterlistload ul li a").unwrap();
        let chapters: Vec<ComicChapter> = doc
            .select(&link_sel)
            .map(|n| ComicChapter {
                title: n.text().collect::<String>().trim().to_string(),
                target_url: format!(
                    "http://www.bikabika.com{}",
                    n.value().attr("href").unwrap_or("")
                ),
            })
            .collect();
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
        headers.insert("User-Agent".into(), "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1".into());
        let settings = RequestSettings {
            address: target_url.into(),
            referrer: Some("http://www.bikabika.com/".into()),
            headers: Some(headers),
            ..Default::default()
        };
        let str_ = self.network.get_string(&settings).await?;
        let re = regex::Regex::new(r#"var qTcms_S_m_murl_e="([^"]+)""#).unwrap();
        if let Some(caps) = re.captures(&str_) {
            let encoded = caps.get(1).map(|m| m.as_str()).unwrap_or("");
            if let Ok(decoded) = base64::engine::general_purpose::STANDARD.decode(encoded) {
                if let Ok(url_str) = String::from_utf8(decoded) {
                    let urls: Vec<&str> = url_str
                        .split("$qingtiandy$")
                        .filter(|s| !s.is_empty())
                        .collect();
                    let pages: Vec<ComicPage> = urls
                        .iter()
                        .rev()
                        .enumerate()
                        .map(|(i, url)| ComicPage {
                            name: (i + 1).to_string(),
                            target_url: url.to_string(),
                        })
                        .collect();
                    return Ok(pages);
                }
            }
        }
        Ok(Vec::new())
    }

    async fn get_image_stream(&self, target_url: &str) -> anf_core::Result<Bytes> {
        let mut headers = std::collections::HashMap::new();
        headers.insert("User-Agent".into(), "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1".into());
        let settings = RequestSettings {
            address: target_url.into(),
            referrer: Some("http://www.bikabika.com/".into()),
            headers: Some(headers),
            ..Default::default()
        };
        self.network.get_stream(&settings).await
    }
}
