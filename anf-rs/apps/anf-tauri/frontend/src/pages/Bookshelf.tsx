import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Loader2, Trash2, Play, RefreshCw } from "lucide-react";
import { getBookshelf, removeFromBookshelf, type BookshelfItem } from "../api";

export default function Bookshelf() {
  const navigate = useNavigate();
  const [items, setItems] = useState<BookshelfItem[]>([]);
  const [loading, setLoading] = useState(true);

  const load = async () => {
    setLoading(true);
    try { setItems(await getBookshelf()); }
    catch (e) { console.error(e); }
    finally { setLoading(false); }
  };

  useEffect(() => { load(); }, []);

  const handleRemove = async (url: string) => {
    try {
      await removeFromBookshelf(url);
      setItems((prev) => prev.filter((i) => i.url !== url));
    } catch (e) { console.error(e); }
  };

  const handleRead = (item: BookshelfItem) => {
    navigate(`/comic?url=${encodeURIComponent(item.url)}`);
  };

  if (loading) {
    return <div className="flex items-center justify-center h-96"><Loader2 className="animate-spin text-emerald-400" size={40} /></div>;
  }

  return (
    <div className="max-w-6xl mx-auto p-4">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-xl font-bold">Bookshelf</h1>
        <button onClick={load} className="flex items-center gap-2 px-3 py-1.5 bg-gray-800 hover:bg-gray-700 rounded-lg text-sm">
          <RefreshCw size={14} /> Refresh
        </button>
      </div>
      {items.length === 0 ? (
        <div className="text-center text-gray-500 py-16">
          <p className="text-lg mb-2">Your bookshelf is empty</p>
          <p className="text-sm">Search for comics and add them to your favorites</p>
        </div>
      ) : (
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
          {items.map((item) => (
            <div key={item.url} className="group relative rounded-lg overflow-hidden bg-gray-800">
              <div className="aspect-[3/4] overflow-hidden bg-gray-700 cursor-pointer" onClick={() => handleRead(item)}>
                {item.image_url ? (
                  <img src={item.image_url} alt={item.name} className="w-full h-full object-cover group-hover:scale-105 transition-transform" loading="lazy" />
                ) : (
                  <div className="flex items-center justify-center h-full text-gray-500 text-sm">No Image</div>
                )}
                {/* Progress bar */}
                {item.chapters_count > 0 && (
                  <div className="absolute bottom-0 inset-x-0 h-1 bg-gray-700">
                    <div className="h-full bg-emerald-500 transition-all"
                      style={{ width: `${Math.min(100, ((item.current_chapter + 1) / item.chapters_count) * 100)}%` }} />
                  </div>
                )}
              </div>
              <div className="p-2">
                <h3 className="text-sm font-medium truncate">{item.name}</h3>
                <p className="text-xs text-gray-500 mt-0.5">
                  Ch. {item.current_chapter + 1}/{item.chapters_count} - Page {item.current_page + 1}
                </p>
              </div>
              {/* Actions overlay */}
              <div className="absolute top-2 right-2 flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                <button onClick={() => handleRead(item)} className="p-1.5 bg-emerald-600 rounded-full hover:bg-emerald-700">
                  <Play size={14} />
                </button>
                <button onClick={() => handleRemove(item.url)} className="p-1.5 bg-red-600 rounded-full hover:bg-red-700">
                  <Trash2 size={14} />
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
