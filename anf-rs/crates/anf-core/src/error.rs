use thiserror::Error;

#[derive(Error, Debug)]
pub enum AnfError {
    #[error("network error: {0}")]
    Network(#[from] reqwest::Error),

    #[error("io error: {0}")]
    Io(#[from] std::io::Error),

    #[error("json error: {0}")]
    Json(#[from] serde_json::Error),

    #[error("invalid url: {0}")]
    InvalidUrl(String),

    #[error("provider not found for url: {0}")]
    ProviderNotFound(String),

    #[error("comic not found: {0}")]
    ComicNotFound(String),

    #[error("parse error: {0}")]
    Parse(String),

    #[error("js eval error: {0}")]
    JsEval(String),

    #[error("cancelled")]
    Cancelled,

    #[error("{0}")]
    Other(String),
}

pub type Result<T> = std::result::Result<T, AnfError>;
