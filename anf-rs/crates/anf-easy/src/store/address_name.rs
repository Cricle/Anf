use md5::{Digest, Md5};

/// AddressToFileNameProvider trait
/// Matches C# IAddressToFileNameProvider
pub trait AddressToFileNameProvider: Send + Sync {
    fn convert(&self, address: &str) -> String;
}

/// MD5-based address to filename provider
/// Matches C# MD5AddressToFileNameProvider
pub struct Md5AddressProvider;

impl AddressToFileNameProvider for Md5AddressProvider {
    fn convert(&self, address: &str) -> String {
        let hash = Md5::digest(address.as_bytes());
        format!("{hash:X}")
    }
}

/// Direct address to filename provider (sanitized)
/// Matches C# DirectAddressToFileNameProvider
pub struct DirectAddressProvider;

impl AddressToFileNameProvider for DirectAddressProvider {
    fn convert(&self, address: &str) -> String {
        sanitize_filename(address)
    }
}

/// Ensure a string is safe for use as a filename
/// Matches C# PathHelper.EnsureName
pub fn sanitize_filename(name: &str) -> String {
    name.chars()
        .map(|c| match c {
            '/' | '\\' | ':' | '*' | '?' | '"' | '<' | '>' | '|' => '_',
            c => c,
        })
        .collect()
}
