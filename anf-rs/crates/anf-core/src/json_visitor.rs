use serde_json::Value;

/// JsonVisitor wraps serde_json::Value for convenient navigation,
/// mirroring the C# JsonVisitor API: visitor["key"].to_string(), visitor["arr"].to_vec()
#[derive(Debug, Clone)]
pub struct JsonVisitor {
    value: Value,
}

impl JsonVisitor {
    pub fn from_str(s: &str) -> crate::Result<Self> {
        let value: Value = if s.trim().is_empty() {
            Value::Null
        } else {
            serde_json::from_str(s)?
        };
        Ok(Self { value })
    }

    pub fn from_value(value: Value) -> Self {
        Self { value }
    }

    pub fn as_value(&self) -> &Value {
        &self.value
    }

    pub fn into_value(self) -> Value {
        self.value
    }

    /// Navigate into an object by key
    pub fn get(&self, key: &str) -> JsonVisitor {
        match &self.value {
            Value::Object(map) => JsonVisitor {
                value: map.get(key).cloned().unwrap_or(Value::Null),
            },
            _ => JsonVisitor { value: Value::Null },
        }
    }

    /// Navigate into an array by index
    pub fn index(&self, i: usize) -> JsonVisitor {
        match &self.value {
            Value::Array(arr) => JsonVisitor {
                value: arr.get(i).cloned().unwrap_or(Value::Null),
            },
            _ => JsonVisitor { value: Value::Null },
        }
    }

    pub fn is_null(&self) -> bool {
        self.value.is_null()
    }

    pub fn is_array(&self) -> bool {
        self.value.is_array()
    }

    /// Iterate over array elements
    pub fn to_vec(&self) -> Vec<JsonVisitor> {
        match &self.value {
            Value::Array(arr) => arr
                .iter()
                .map(|v| JsonVisitor { value: v.clone() })
                .collect(),
            _ => Vec::new(),
        }
    }

    /// Get string value
    pub fn as_str(&self) -> Option<&str> {
        match &self.value {
            Value::String(s) => Some(s),
            _ => None,
        }
    }

    /// Get i64 value
    pub fn as_i64(&self) -> Option<i64> {
        self.value.as_i64()
    }

    /// Get f64 value
    pub fn as_f64(&self) -> Option<f64> {
        self.value.as_f64()
    }

    /// Get bool value
    pub fn as_bool(&self) -> Option<bool> {
        self.value.as_bool()
    }

    /// Get array length
    pub fn len(&self) -> usize {
        match &self.value {
            Value::Array(arr) => arr.len(),
            Value::Object(map) => map.len(),
            _ => 0,
        }
    }

    pub fn is_empty(&self) -> bool {
        self.len() == 0
    }

    /// Serialize back to string
    pub fn to_json_string(&self) -> String {
        serde_json::to_string(&self.value).unwrap_or_default()
    }
}

impl std::fmt::Display for JsonVisitor {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match &self.value {
            Value::String(s) => write!(f, "{s}"),
            Value::Null => write!(f, ""),
            other => write!(f, "{other}"),
        }
    }
}

/// Index by &str
impl<'a> std::ops::Index<&'a str> for JsonVisitor {
    type Output = Value;
    fn index(&self, key: &'a str) -> &Value {
        match &self.value {
            Value::Object(map) => map.get(key).unwrap_or(&Value::Null),
            _ => &Value::Null,
        }
    }
}
