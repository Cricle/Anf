/// Fast host extraction matching C# UrlHelper.FastGetHost
pub fn fast_get_host(address: &str) -> &str {
    let bytes = address.as_bytes();
    let len = bytes.len();
    let mut start = 0usize;
    let mut end = len;
    let mut i = 0;
    while i < len {
        let c = bytes[i];
        if c == b'/' || c == b'?' {
            end = i;
            break;
        } else if c == b':' && (len - i) > 3 && bytes[i + 1] == b'/' && bytes[i + 2] == b'/' {
            start = i + 3;
            i = start;
        }
        i += 1;
    }
    &address[start..end]
}

pub fn is_website(s: &str) -> bool {
    s.starts_with("http://") || s.starts_with("https://") || s.starts_with("www.")
}

pub fn get_url(s: &str) -> &str {
    if s.starts_with("www.") {
        // can't return owned from &str without allocation; caller should use format!
        s
    } else {
        s
    }
}

pub fn ensure_url(s: &str) -> String {
    if s.starts_with("www.") {
        format!("http://{s}")
    } else {
        s.to_string()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_fast_get_host() {
        assert_eq!(
            fast_get_host("https://manga.bilibili.com/detail/mc123"),
            "manga.bilibili.com"
        );
        assert_eq!(
            fast_get_host("http://www.dm5.com/comic/abc/"),
            "www.dm5.com"
        );
        assert_eq!(fast_get_host("https://example.com"), "example.com");
    }

    #[test]
    fn test_is_website() {
        assert!(is_website("http://example.com"));
        assert!(is_website("https://example.com"));
        assert!(is_website("www.example.com"));
        assert!(!is_website("ftp://example.com"));
    }
}
