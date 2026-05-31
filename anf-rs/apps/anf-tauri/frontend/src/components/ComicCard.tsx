import type { ComicSnapshot } from "../api";

interface Props {
  snapshot: ComicSnapshot;
  onClick: () => void;
  wide?: boolean;
}

export default function ComicCard({ snapshot, onClick, wide }: Props) {
  return (
    <div
      onClick={onClick}
      className={`group cursor-pointer rounded-lg overflow-hidden bg-gray-800 hover:ring-2 hover:ring-emerald-500 transition-all ${wide ? "min-w-[240px]" : ""}`}
    >
      <div className="relative aspect-[3/4] overflow-hidden bg-gray-700">
        {snapshot.image_uri ? (
          <img src={snapshot.image_uri} alt={snapshot.name}
            className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300" loading="lazy" />
        ) : (
          <div className="flex items-center justify-center h-full text-gray-500 text-sm">No Image</div>
        )}
        <div className="absolute bottom-0 inset-x-0 bg-gradient-to-t from-black/80 to-transparent p-3">
          <h3 className="text-sm font-medium text-white line-clamp-2">{snapshot.name}</h3>
          {snapshot.author && <p className="text-xs text-gray-300 mt-1">{snapshot.author}</p>}
        </div>
      </div>
      {snapshot.sources.length > 0 && (
        <div className="flex gap-1 p-2">
          {snapshot.sources.map((s, i) => (
            <span key={i} className="text-[10px] px-1.5 py-0.5 bg-gray-700 rounded text-gray-400">{s.name}</span>
          ))}
        </div>
      )}
    </div>
  );
}
