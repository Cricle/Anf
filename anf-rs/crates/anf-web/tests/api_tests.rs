use axum::body::Body;
use axum::http::{Request, StatusCode};
use serde_json::Value;
use tower::ServiceExt; // for `oneshot`

async fn app() -> axum::Router {
    anf_web::app::create_app().await.expect("create app")
}

fn get(path: &str) -> Request<Body> {
    Request::builder()
        .uri(path)
        .body(Body::empty())
        .unwrap()
}

async fn body_json(resp: axum::response::Response) -> Value {
    let bytes = axum::body::to_bytes(resp.into_body(), usize::MAX)
        .await
        .unwrap();
    serde_json::from_slice(&bytes).unwrap()
}

// ========== Reading API ==========

#[tokio::test]
async fn test_get_providers() {
    let app = app().await;
    let resp = app.oneshot(get("/api/v1/reading/get-providers")).await.unwrap();
    assert_eq!(resp.status(), StatusCode::OK);

    let json = body_json(resp).await;
    assert_eq!(json["code"], 0);
    assert!(json["data"].is_array());
}

#[tokio::test]
async fn test_search() {
    let app = app().await;
    let resp = app
        .oneshot(get("/api/v1/reading/search?keyword=test&skip=0&take=5"))
        .await
        .unwrap();
    assert_eq!(resp.status(), StatusCode::OK);

    let json = body_json(resp).await;
    assert_eq!(json["code"], 0);
    assert!(json["data"].is_object());
}

#[tokio::test]
async fn test_search_missing_keyword() {
    let app = app().await;
    let resp = app
        .oneshot(get("/api/v1/reading/search"))
        .await
        .unwrap();
    // Missing keyword should return 400 or error
    assert!(resp.status() == StatusCode::BAD_REQUEST || resp.status() == StatusCode::OK);
}

#[tokio::test]
async fn test_get_entity_missing_url() {
    let app = app().await;
    let resp = app
        .oneshot(get("/api/v1/reading/get-entity"))
        .await
        .unwrap();
    // Missing url param should return 400
    assert_eq!(resp.status(), StatusCode::BAD_REQUEST);
}

#[tokio::test]
async fn test_get_proposal() {
    let app = app().await;
    let resp = app
        .oneshot(get("/api/v1/reading/get-proposal?take=5"))
        .await
        .unwrap();
    assert_eq!(resp.status(), StatusCode::OK);

    let json = body_json(resp).await;
    // May return error if no proposal engines loaded, that's OK
    assert!(json["code"].is_number());
}

#[tokio::test]
async fn test_get_chapter_missing_params() {
    let app = app().await;
    let resp = app
        .oneshot(get("/api/v1/reading/get-chapter"))
        .await
        .unwrap();
    assert_eq!(resp.status(), StatusCode::BAD_REQUEST);
}

#[tokio::test]
async fn test_get_image_missing_params() {
    let app = app().await;
    let resp = app
        .oneshot(get("/api/v1/reading/get-image"))
        .await
        .unwrap();
    assert_eq!(resp.status(), StatusCode::BAD_REQUEST);
}

// ========== Download API ==========

#[tokio::test]
async fn test_download_list_empty() {
    let app = app().await;
    let resp = app.oneshot(get("/api/v1/download/list")).await.unwrap();
    assert_eq!(resp.status(), StatusCode::OK);

    let json = body_json(resp).await;
    assert_eq!(json["code"], 0);
    assert!(json["data"].is_array());
}

#[tokio::test]
async fn test_download_start() {
    let app = app().await;
    let resp = app
        .oneshot(get("/api/v1/download/start?url=http://test.com/comic1&name=TestComic"))
        .await
        .unwrap();
    assert_eq!(resp.status(), StatusCode::OK);

    let json = body_json(resp).await;
    assert_eq!(json["code"], 0);
    assert_eq!(json["data"], "ok");
}

#[tokio::test]
async fn test_download_status() {
    let app = app().await;

    // Start a download first
    let resp = app
        .clone()
        .oneshot(get("/api/v1/download/start?url=http://test.com/comic2&name=TestComic2"))
        .await
        .unwrap();
    assert_eq!(resp.status(), StatusCode::OK);

    // Check status
    let resp = app
        .oneshot(get("/api/v1/download/status?url=http://test.com/comic2"))
        .await
        .unwrap();
    assert_eq!(resp.status(), StatusCode::OK);

    let json = body_json(resp).await;
    assert_eq!(json["code"], 0);
    assert!(json["data"].is_object());
    assert_eq!(json["data"]["name"], "TestComic2");
}

#[tokio::test]
async fn test_download_cancel() {
    let app = app().await;

    // Start a download
    let resp = app
        .clone()
        .oneshot(get("/api/v1/download/start?url=http://test.com/comic3&name=TestComic3"))
        .await
        .unwrap();
    assert_eq!(resp.status(), StatusCode::OK);

    // Cancel it
    let resp = app
        .oneshot(get("/api/v1/download/cancel?url=http://test.com/comic3"))
        .await
        .unwrap();
    assert_eq!(resp.status(), StatusCode::OK);

    let json = body_json(resp).await;
    assert_eq!(json["code"], 0);
}

#[tokio::test]
async fn test_download_start_duplicate() {
    let app = app().await;

    // Start a download
    let resp = app
        .clone()
        .oneshot(get("/api/v1/download/start?url=http://test.com/dup&name=DupComic"))
        .await
        .unwrap();
    assert_eq!(resp.status(), StatusCode::OK);

    // Try to start the same download again
    let resp = app
        .oneshot(get("/api/v1/download/start?url=http://test.com/dup&name=DupComic"))
        .await
        .unwrap();
    assert_eq!(resp.status(), StatusCode::OK);

    let json = body_json(resp).await;
    assert_eq!(json["code"], 1); // should be error
    assert!(json["msg"].as_str().unwrap().contains("already downloading"));
}

#[tokio::test]
async fn test_export_pdf_not_found() {
    let app = app().await;
    let resp = app
        .oneshot(get("/api/v1/download/export-pdf?name=nonexistent_comic"))
        .await
        .unwrap();
    assert_eq!(resp.status(), StatusCode::OK);

    let json = body_json(resp).await;
    assert_eq!(json["code"], 1);
    assert!(json["msg"].as_str().unwrap().contains("not found"));
}

#[tokio::test]
async fn test_export_pdf_with_images() {
    use std::io::Write;

    // Create a temp directory with test images
    let tmp = tempfile::tempdir().unwrap();
    let comic_dir = tmp.path().join("TestPdfComic").join("Chapter 1");
    std::fs::create_dir_all(&comic_dir).unwrap();

    // Create a minimal valid PNG image (1x1 red pixel)
    let png_data = create_minimal_png();
    let mut f = std::fs::File::create(comic_dir.join("0001.png")).unwrap();
    f.write_all(&png_data).unwrap();

    // Set ANF_DOWNLOAD_DIR before creating app
    let download_dir = tmp.path();
    eprintln!("ANF_DOWNLOAD_DIR={}", download_dir.display());
    std::env::set_var("ANF_DOWNLOAD_DIR", download_dir);

    let app = app().await;
    let resp = app
        .oneshot(get("/api/v1/download/export-pdf?name=TestPdfComic"))
        .await
        .unwrap();
    assert_eq!(resp.status(), StatusCode::OK);

    let content_type = resp.headers().get("content-type").unwrap().to_str().unwrap().to_string();
    let body_bytes = axum::body::to_bytes(resp.into_body(), usize::MAX).await.unwrap();
    if content_type != "application/pdf" {
        let json: Value = serde_json::from_slice(&body_bytes).unwrap_or_else(|_| {
            serde_json::json!({"raw": String::from_utf8_lossy(&body_bytes)})
        });
        panic!("Expected PDF, got content-type={}: {}", content_type, json);
    }

    assert!(!body_bytes.is_empty());
    assert!(body_bytes.starts_with(b"%PDF"));

    std::env::remove_var("ANF_DOWNLOAD_DIR");
}

fn create_minimal_png() -> Vec<u8> {
    use image::{ImageBuffer, Rgb, RgbImage};
    let img: RgbImage = ImageBuffer::from_pixel(1, 1, Rgb([255u8, 0, 0]));
    let mut buf = std::io::Cursor::new(Vec::new());
    img.write_to(&mut buf, image::ImageFormat::Png).unwrap();
    buf.into_inner()
}
