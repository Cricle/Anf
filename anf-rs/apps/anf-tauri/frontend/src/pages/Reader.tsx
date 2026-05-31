import { useEffect, useState, useRef, useCallback } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import {
  Loader2, ChevronLeft, ChevronRight, List, X, ZoomIn, ZoomOut,
  Maximize, ArrowLeft, Columns, Rows
} from "lucide-react";
import { useStore } from "../store";
import { getChapter, getImage, getEntity, updateReadingProgress, type WithPageChapter, type ComicEntityTruck } from "../api";

type ViewMode = "vertical" | "paged";

export default function Reader() {
  const [params] = useSearchParams();
  const navigate = useNavigate();
  const entityUrl = params.get("url") || "";
  const initialChapter = parseInt(params.get("chapter") || "0", 10);
  const initialChapterUrl = params.get("chapterUrl") || "";

  const [comic, setComic] = useState<ComicEntityTruck | null>(null);
  const [chapter, setChapter] = useState<WithPageChapter | null>(null);
  const [chapterIdx, setChapterIdx] = useState(initialChapter);
  const [images, setImages] = useState<Map<string, string>>(new Map());
  const [loading, setLoading] = useState(true);
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [viewMode, setViewMode] = useState<ViewMode>("vertical");
  const [zoom, setZoom] = useState(1);
  const [pageIdx, setPageIdx] = useState(0);
  const containerRef = useRef<HTMLDivElement>(null);

  // Load comic entity for sidebar
  useEffect(() => {
    if (!entityUrl) return;
    getEntity(entityUrl).then(setComic).catch(console.error);
  }, [entityUrl]);

  // Load chapter
  const loadChapter = useCallback(async (url: string, idx: number) => {
    setLoading(true);
    setImages(new Map());
    setPageIdx(0);
    try {
      const ch = await getChapter(entityUrl, url);
      setChapter(ch);
      setChapterIdx(idx);

      // Load all images
      const newImages = new Map<string, string>();
      await Promise.all(ch.pages.map(async (page) => {
        try {
          const data = await getImage(entityUrl, page.target_url);
          const blob = new Blob([new Uint8Array(data)], { type: "image/png" });
          newImages.set(page.target_url, URL.createObjectURL(blob));
        } catch (e) {
          console.error("Failed to load image:", e);
        }
      }));
      setImages(newImages);

      // Save progress
      if (entityUrl) {
        updateReadingProgress(entityUrl, idx, 0).catch(console.error);
      }
    } catch (e) { console.error(e); }
    finally { setLoading(false); }
  }, [entityUrl]);

  useEffect(() => {
    if (initialChapterUrl) {
      loadChapter(initialChapterUrl, initialChapter);
    }
  }, [initialChapterUrl, initialChapter, loadChapter]);

  const goToChapter = (idx: number) => {
    if (!comic || idx < 0 || idx >= comic.chapters.length) return;
    loadChapter(comic.chapters[idx].target_url, idx);
    setSidebarOpen(false);
  };

  const prevPage = () => setPageIdx((p) => Math.max(0, p - 1));
  const nextPage = () => setPageIdx((p) => Math.min((chapter?.pages.length || 1) - 1, p + 1));

  // Keyboard navigation
  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if (viewMode === "paged") {
        if (e.key === "ArrowLeft" || e.key === "a") prevPage();
        if (e.key === "ArrowRight" || e.key === "d") nextPage();
      }
    };
    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, [viewMode, chapter]);

  const imageList = chapter?.pages.map((p) => images.get(p.target_url)).filter(Boolean) as string[] || [];

  return (
    <div className="flex h-screen bg-gray-950">
      {/* Sidebar */}
      {sidebarOpen && comic && (
        <aside className="w-64 flex-shrink-0 bg-gray-900 border-r border-gray-800 flex flex-col overflow-hidden">
          <div className="flex items-center justify-between px-3 py-2 border-b border-gray-800">
            <span className="text-sm font-medium truncate">{comic.name}</span>
            <button onClick={() => setSidebarOpen(false)}><X size={16} /></button>
          </div>
          <div className="flex-1 overflow-auto">
            {comic.chapters.map((ch, i) => (
              <button key={i} onClick={() => goToChapter(i)}
                className={`w-full text-left px-3 py-1.5 text-sm truncate transition-colors ${i === chapterIdx ? "bg-emerald-600/30 text-emerald-400" : "hover:bg-gray-800 text-gray-300"}`}>
                {ch.title}
              </button>
            ))}
          </div>
        </aside>
      )}

      {/* Main content */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Toolbar */}
        <header className="flex items-center gap-2 px-3 py-1.5 bg-gray-900 border-b border-gray-800 text-sm">
          <button onClick={() => navigate(entityUrl ? `/comic?url=${encodeURIComponent(entityUrl)}` : "/")}
            className="hover:text-emerald-400"><ArrowLeft size={18} /></button>
          <button onClick={() => setSidebarOpen(!sidebarOpen)} className="hover:text-emerald-400"><List size={18} /></button>
          <span className="truncate flex-1 text-gray-400">{chapter?.title || "Loading..."}</span>
          <span className="text-gray-500 text-xs">
            {chapterIdx + 1}/{comic?.chapters.length || "?"}
          </span>
          <div className="flex items-center gap-1 ml-2">
            <button onClick={() => goToChapter(chapterIdx - 1)} className="hover:text-emerald-400 p-1"><ChevronLeft size={16} /></button>
            <button onClick={() => goToChapter(chapterIdx + 1)} className="hover:text-emerald-400 p-1"><ChevronRight size={16} /></button>
          </div>
          <div className="w-px h-4 bg-gray-700 mx-1" />
          <button onClick={() => setViewMode(viewMode === "vertical" ? "paged" : "vertical")}
            className="hover:text-emerald-400 p-1" title={viewMode === "vertical" ? "Switch to paged" : "Switch to vertical"}>
            {viewMode === "vertical" ? <Columns size={16} /> : <Rows size={16} />}
          </button>
          <button onClick={() => setZoom((z) => Math.max(0.25, z - 0.25))} className="hover:text-emerald-400 p-1"><ZoomOut size={16} /></button>
          <span className="text-xs w-10 text-center">{Math.round(zoom * 100)}%</span>
          <button onClick={() => setZoom((z) => Math.min(4, z + 0.25))} className="hover:text-emerald-400 p-1"><ZoomIn size={16} /></button>
          <button onClick={() => setZoom(1)} className="hover:text-emerald-400 p-1"><Maximize size={16} /></button>
        </header>

        {/* Images */}
        <div ref={containerRef} className="flex-1 overflow-auto" style={{ transform: `scale(${zoom})`, transformOrigin: "top center" }}>
          {loading ? (
            <div className="flex items-center justify-center h-full"><Loader2 className="animate-spin text-emerald-400" size={40} /></div>
          ) : viewMode === "vertical" ? (
            <div className="flex flex-col items-center py-4 gap-2">
              {imageList.map((src, i) => (
                <img key={i} src={src} alt={`Page ${i + 1}`} className="max-w-full" loading="lazy" />
              ))}
              {imageList.length === 0 && <div className="text-gray-500 py-12">No images loaded</div>}
              {/* Next chapter */}
              {comic && chapterIdx + 1 < comic.chapters.length && (
                <button onClick={() => goToChapter(chapterIdx + 1)}
                  className="mt-4 px-6 py-3 bg-emerald-600 hover:bg-emerald-700 rounded-lg font-medium">
                  Next Chapter: {comic.chapters[chapterIdx + 1].title}
                </button>
              )}
            </div>
          ) : (
            <div className="flex items-center justify-center h-full relative">
              <button onClick={prevPage} className="absolute left-4 top-1/2 -translate-y-1/2 p-2 bg-gray-800/60 rounded-full hover:bg-gray-700">
                <ChevronLeft size={24} />
              </button>
              {imageList[pageIdx] ? (
                <img src={imageList[pageIdx]} alt={`Page ${pageIdx + 1}`} className="max-h-full max-w-full object-contain" />
              ) : (
                <div className="text-gray-500">No image</div>
              )}
              <button onClick={nextPage} className="absolute right-4 top-1/2 -translate-y-1/2 p-2 bg-gray-800/60 rounded-full hover:bg-gray-700">
                <ChevronRight size={24} />
              </button>
              <div className="absolute bottom-4 left-1/2 -translate-x-1/2 bg-gray-900/80 px-3 py-1 rounded-full text-sm">
                {pageIdx + 1} / {imageList.length}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
