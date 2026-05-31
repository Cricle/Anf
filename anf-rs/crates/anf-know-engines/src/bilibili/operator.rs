use anf_core::{
    ComicChapter, ComicEntity, ComicInfo, ComicPage, ComicSourceProvider, JsonVisitor,
    NetworkAdapter, RequestSettings,
};
use async_trait::async_trait;
use bytes::Bytes;
use std::sync::Arc;

const DETAIL_URI: &str =
    "https://manga.bilibili.com/twirp/comic.v1.Comic/ComicDetail?device=pc&platform=web";
const IMG_INDEX_URI: &str =
    "https://manga.bilibili.com/twirp/comic.v1.Comic/GetImageIndex?device=pc&platform=web";
const IMG_TOKEN_URI: &str =
    "https://manga.bilibili.com/twirp/comic.v1.Comic/ImageToken?device=pc&platform=web";

pub struct BilibiliProvider {
    network: Arc<dyn NetworkAdapter>,
}

impl BilibiliProvider {
    pub fn new(network: Arc<dyn NetworkAdapter>) -> Self {
        Self { network }
    }

    fn json_headers() -> std::collections::HashMap<String, String> {
        let mut h = std::collections::HashMap::new();
        h.insert("Content-Type".into(), "application/json".into());
        h.insert("User-Agent".into(), "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1".into());
        h
    }

    async fn post_json(&self, url: &str, body: &str) -> anf_core::Result<String> {
        let settings = RequestSettings {
            address: url.to_string(),
            host: Some("manga.bilibili.com".into()),
            referrer: Some("https://manga.bilibili.com/".into()),
            method: Some("POST".into()),
            data: Some(Bytes::from(body.to_string())),
            headers: Some(Self::json_headers()),
            ..Default::default()
        };
        self.network.get_string(&settings).await
    }
}

#[async_trait]
impl ComicSourceProvider for BilibiliProvider {
    async fn get_chapters(&self, target_url: &str) -> anf_core::Result<ComicEntity> {
        let mc = target_url.rsplit('/').next().unwrap_or("");
        let part = &mc[2..]; // remove "mc"
        let body = format!(r#"{{"comic_id":{}}}"#, part);
        let str_ = self.post_json(DETAIL_URI, &body).await?;
        let jv = JsonVisitor::from_str(&str_)?;
        let data = jv.get("data");
        let entity = ComicEntity {
            info: ComicInfo {
                comic_url: target_url.to_string(),
                name: data.get("title").to_string(),
                descript: data.get("evaluate").to_string(),
                image_url: data.get("vertical_cover").to_string(),
            },
            chapters: {
                let ep = data.get("ep_list").to_vec();
                let mut chapts: Vec<ComicChapter> = ep
                    .iter()
                    
                    .map(|item| {
                        let title = item.get("title").to_string();
                        let title = if title.trim().is_empty() {
                            format!("{}-{}", data.get("title"), ep.len())
                        } else {
                            title
                        };
                        let id = item.get("id").to_string();
                        ComicChapter {
                            target_url: format!("https://manga.bilibili.com/{}/mc{}", mc, id),
                            title,
                        }
                    })
                    .collect();
                chapts.reverse();
                chapts
            },
        };
        Ok(entity)
    }

    async fn get_pages(&self, target_url: &str) -> anf_core::Result<Vec<ComicPage>> {
        let width = 660;
        let ep_id = target_url
            .rsplit('/')
            .next()
            .unwrap_or("")
            .trim_start_matches(['m', 'c']);
        let body = format!(r#"{{"ep_id":{}}}"#, ep_id);
        let str_ = self.post_json(IMG_INDEX_URI, &body).await?;
        let jv = JsonVisitor::from_str(&str_)?;
        let imgs = jv.get("data").get("images").to_vec();
        let paths: Vec<String> = imgs
            .iter()
            .map(|x| format!("{}@{}w.jpg", x.get("path"), width))
            .collect();
        let urls_json = format!(
            r#"{{"urls":"[{}]"}}"#,
            paths
                .iter()
                .map(|p| format!("\\\"{}\\\"", p))
                .collect::<Vec<_>>()
                .join(",")
        );
        let str2 = self.post_json(IMG_TOKEN_URI, &urls_json).await?;
        let jv2 = JsonVisitor::from_str(&str2)?;
        let data = jv2.get("data").to_vec();
        let pages: Vec<ComicPage> = data
            .iter()
            .enumerate()
            .map(|(i, x)| ComicPage {
                name: i.to_string(),
                target_url: format!(
                    "{}?token={}",
                    x.get("url"),
                    x.get("token")
                ),
            })
            .collect();
        Ok(pages)
    }

    async fn get_image_stream(&self, target_url: &str) -> anf_core::Result<Bytes> {
        let settings = RequestSettings {
            address: target_url.to_string(),
            host: Some("manga.bilibili.com".into()),
            referrer: Some("https://manga.bilibili.com/".into()),
            headers: Some(Self::json_headers()),
            ..Default::default()
        };
        self.network.get_stream(&settings).await
    }
}
