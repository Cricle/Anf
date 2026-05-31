import { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { Loader2, Heart, Play, ExternalLink, Copy } from "lucide-react";
import { getEntity, addToBookshelf, type ComicEntityTruck } from "../api";

export default function ComicDetail() {
  const [params] = useSearchParams();
  const navigate = useNavigate();
  const url = params.get("url") || "";
  const [comic, setComic] = useState<ComicEntityTruck | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!url) return;
    setLoading(true);
    setError("");
    getEntity(url)
      .then(setComic)
      .catch((e) => setError(String(e)))
      .finally(() => setLoading(false));
  }, [url]);

  const handleRead = (chapterUrl: string, idx: number) => {
    navigate(`/read?url=${encodeURIComponent(url)}&chapter=${idx}&chapterUrl=${encodeURIComponent(chapterUrl)}`);
  };

  const handleFavorite = async () => {
    if (!comic) return;
    try {
      await addToBookshelf({
        url: comic.comic_url,
        name: comic.name,
        image_url: comic.image_url,
        descript: comic.descript,
        chapters_count: comic.chapters.length,
      });
    } catch (e) { console.error(e); }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-96">
        <Loader2 className="animate-spin text-emerald-400" size={40} />
      </div>
    );
  }

  if (error) {
    return <div className="text-center text-red-400 py-12">{error}</div>;
  }

  if (!comic) return null;

  return (
    <div className="max-w-5xl mx-auto p-4">
      {/* Header */}
      <div className="flex gap-6 mb-8">
        <div className="w-56 flex-shrink-0 rounded-lg overflow-hidden bg-gray-800">
          {comic.image_url ? (
            <img src={comic.image_url} alt={comic.name} className="w-full aspect-[3/4] object-cover" />
          ) : (
            <div className="w-full aspect-[3/4] flex items-center justify-center text-gray-500">No Cover</div>
          )}
        </div>
        <div className="flex-1 flex flex-col">
          <h1 className="text-2xl font-bold mb-2">{comic.name}</h1>
          <p className="text-gray-400 text-sm mb-4 leading-relaxed max-h-40 overflow-auto">{comic.descript || "No description"}</p>
          <div className="flex items-center gap-2 text-xs text-gray-500 mb-4">
            <span>{comic.chapters.length} chapters</span>
            {comic.chapters.length > 0 && (
              <>
                <span className="text-gray-700">|</span>
                <span>{comic.chapters[0].title}</span>
              </>
            )}
          </div>
          <div className="flex gap-2 mt-auto">
            {comic.chapters.length > 0 && (
              <button onClick={() => handleRead(comic.chapters[0].target_url, 0)}
                className="flex items-center gap-2 px-5 py-2 bg-emerald-600 hover:bg-emerald-700 rounded-lg font-medium">
                <Play size={16} /> Read
              </button>
            )}
            <button onClick={handleFavorite}
              className="flex items-center gap-2 px-4 py-2 bg-gray-700 hover:bg-gray-600 rounded-lg">
              <Heart size={16} /> Favorite
            </button>
            <button onClick={() => navigator.clipboard.writeText(url)}
              className="flex items-center gap-2 px-4 py-2 bg-gray-700 hover:bg-gray-600 rounded-lg">
              <Copy size={16} /> Copy URL
            </button>
          </div>
        </div>
      </div>

      {/* Chapters */}
      <h2 className="text-lg font-semibold mb-3">Chapters</h2>
      <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-2">
        {comic.chapters.map((ch, i) => (
          <button key={i} onClick={() => handleRead(ch.target_url, i)}
            className="text-left px-3 py-2 bg-gray-800 hover:bg-gray-700 rounded text-sm truncate transition-colors">
            {ch.title}
          </button>
        ))}
      </div>
    </div>
  );
}
