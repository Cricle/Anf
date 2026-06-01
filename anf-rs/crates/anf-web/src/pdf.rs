use printpdf::*;
use std::io::BufWriter;
use std::path::Path;

pub async fn comic_to_pdf(comic_dir: &Path) -> Result<Vec<u8>, String> {
    let mut chapter_dirs: Vec<_> = std::fs::read_dir(comic_dir)
        .map_err(|e| format!("read comic dir: {e}"))?
        .filter_map(|e| e.ok())
        .filter(|e| e.path().is_dir())
        .collect();
    chapter_dirs.sort_by_key(|e| e.file_name());

    if chapter_dirs.is_empty() {
        return Err("no chapters found".into());
    }

    let doc = PdfDocument::empty("comic");

    for ch_dir in &chapter_dirs {
        let mut images: Vec<_> = std::fs::read_dir(ch_dir.path())
            .map_err(|e| format!("read chapter dir: {e}"))?
            .filter_map(|e| e.ok())
            .filter(|e| {
                let p = e.path();
                p.is_file()
                    && p.extension()
                        .map(|ext| {
                            matches!(
                                ext.to_str().unwrap_or(""),
                                "jpg" | "jpeg" | "png" | "gif" | "webp" | "bmp"
                            )
                        })
                        .unwrap_or(false)
            })
            .collect();
        images.sort_by_key(|e| e.file_name());

        for img_entry in images {
            let img_path = img_entry.path();
            let dynamic_img = ::image::io::Reader::open(&img_path)
                .map_err(|e| format!("open image: {e}"))?
                .decode()
                .map_err(|e| format!("decode image: {e}"))?;

            let (w, h) = (dynamic_img.width(), dynamic_img.height());

            // Page size: fit image to ~A4 proportions
            let page_w = 595.0_f32;
            let page_h = 842.0_f32;
            let scale_x = page_w / w as f32;
            let scale_y = page_h / h as f32;
            let scale = scale_x.min(scale_y);

            let img = Image::from_dynamic_image(&dynamic_img);

            let (page_idx, layer_idx) = doc.add_page(
                Mm(page_w * 25.4 / 72.0),
                Mm(page_h * 25.4 / 72.0),
                "Layer",
            );
            let layer = doc.get_page(page_idx).get_layer(layer_idx);

            img.add_to_layer(
                layer,
                ImageTransform {
                    translate_x: Some(Mm(0.0)),
                    translate_y: Some(Mm(0.0)),
                    scale_x: Some(scale),
                    scale_y: Some(scale),
                    rotate: None,
                    dpi: Some(72.0),
                },
            );
        }
    }

    let mut buf = Vec::new();
    doc.save(&mut BufWriter::new(&mut buf))
        .map_err(|e| format!("save pdf: {e}"))?;
    Ok(buf)
}
