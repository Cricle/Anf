use serde::{Deserialize, Serialize};

/// Result: API response wrapper
/// Matches C# Result
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Result {
    #[serde(default)]
    pub code: i32,
    #[serde(default)]
    pub msg: Option<String>,
}

impl Result {
    pub fn succeed() -> Self {
        Self { code: 0, msg: None }
    }

    pub fn error(code: i32, msg: impl Into<String>) -> Self {
        Self {
            code,
            msg: Some(msg.into()),
        }
    }

    pub fn is_succeed(&self) -> bool {
        self.code == 0
    }
}

/// EntityResult<T>: API response with data
/// Matches C# EntityResult<T>
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct EntityResult<T> {
    #[serde(default)]
    pub code: i32,
    #[serde(default)]
    pub msg: Option<String>,
    pub data: Option<T>,
}

impl<T> EntityResult<T> {
    pub fn new(data: T) -> Self {
        Self {
            code: 0,
            msg: None,
            data: Some(data),
        }
    }

    pub fn error(code: i32, msg: impl Into<String>) -> Self {
        Self {
            code,
            msg: Some(msg.into()),
            data: None,
        }
    }
}
