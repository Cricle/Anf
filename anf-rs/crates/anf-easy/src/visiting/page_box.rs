use anf_core::ComicPage;

/// PageBox: wraps a page with its loaded resource
/// Matches C# PageBox<TResource> : IComicVisitPage<TResource>
#[derive(Debug, Clone)]
pub struct PageBox<T> {
    pub page: ComicPage,
    pub resource: Option<T>,
}
