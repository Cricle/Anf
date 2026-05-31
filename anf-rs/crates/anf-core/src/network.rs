use async_trait::async_trait;
use bytes::Bytes;
use reqwest::Client;
use std::collections::HashMap;
use std::time::Duration;

use crate::Result;

// ── RequestSettings ────────────────────────────────────────────

#[derive(Debug, Clone, Default)]
pub struct RequestSettings {
    pub address: String,
    pub host: Option<String>,
    pub method: Option<String>,
    pub accept: Option<String>,
    pub referrer: Option<String>,
    pub timeout: Option<Duration>,
    pub data: Option<Bytes>,
    pub headers: Option<HashMap<String, String>>,
}

// ── NetworkAdapter trait ───────────────────────────────────────

#[async_trait]
pub trait NetworkAdapter: Send + Sync {
    async fn get_stream(&self, settings: &RequestSettings) -> Result<Bytes>;

    async fn get_string(&self, settings: &RequestSettings) -> Result<String> {
        let bytes = self.get_stream(settings).await?;
        Ok(String::from_utf8_lossy(&bytes).into_owned())
    }
}

// ── ReqwestAdapter ─────────────────────────────────────────────

pub struct ReqwestAdapter {
    client: Client,
}

impl ReqwestAdapter {
    pub fn new(client: Client) -> Self {
        Self { client }
    }

    pub fn with_default() -> Self {
        let client = Client::builder()
            .timeout(Duration::from_secs(30))
            .redirect(reqwest::redirect::Policy::limited(10))
            .build()
            .expect("failed to build reqwest client");
        Self { client }
    }
}

#[async_trait]
impl NetworkAdapter for ReqwestAdapter {
    async fn get_stream(&self, settings: &RequestSettings) -> Result<Bytes> {
        let method = settings.method.as_deref().unwrap_or("GET");
        let mut req = match method.to_uppercase().as_str() {
            "POST" => self.client.post(&settings.address),
            "PUT" => self.client.put(&settings.address),
            _ => self.client.get(&settings.address),
        };

        // Host
        if let Some(ref host) = settings.host {
            req = req.header("Host", host.as_str());
        }
        // Referrer
        if let Some(ref referrer) = settings.referrer {
            req = req.header("Referer", referrer.as_str());
        }
        // Accept
        if let Some(ref accept) = settings.accept {
            req = req.header("Accept", accept.as_str());
        }
        // Custom headers
        if let Some(ref headers) = settings.headers {
            for (k, v) in headers {
                req = req.header(k.as_str(), v.as_str());
            }
        }
        // Body
        if let Some(ref data) = settings.data {
            req = req.body(data.clone());
        }
        // Timeout
        if let Some(timeout) = settings.timeout {
            req = req.timeout(timeout);
        }

        let resp = req.send().await?;
        let bytes = resp.bytes().await?;
        Ok(bytes)
    }
}
