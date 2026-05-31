import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Search as SearchIcon, Loader2 } from "lucide-react";
import { useStore } from "../store";
import { search as searchApi, getProposal, type ComicSnapshot } from "../api";
import ComicCard from "../components/ComicCard";

export default function HomePage() {
  const navigate = useNavigate();
  const { keyword, setKeyword, searchResults, setSearchResults, searching, setSearching } = useStore();
  const [proposals, setProposals] = useState<ComicSnapshot[]>([]);
  const [proposalLoading, setProposalLoading] = useState(false);

  useEffect(() => {
    setProposalLoading(true);
    getProposal(undefined, 10).then(setProposals).catch(console.error).finally(() => setProposalLoading(false));
  }, []);

  const handleSearch = async () => {
    if (!keyword.trim()) return;
    setSearching(true);
    try {
      const result = await searchApi(keyword);
      setSearchResults(result.snapshots);
    } catch (e) { console.error(e); }
    finally { setSearching(false); }
  };

  const handleClick = (snap: ComicSnapshot) => {
    const url = snap.sources[0]?.target_url || snap.target_url;
    navigate(`/comic?url=${encodeURIComponent(url)}`);
  };

  const showResults = searchResults.length > 0;

  return (
    <div className="max-w-6xl mx-auto p-4">
      <div className="flex gap-2 mb-6">
        <input
          value={keyword}
          onChange={(e) => setKeyword(e.target.value)}
          onKeyDown={(e) => e.key === "Enter" && handleSearch()}
          placeholder="Search comics or paste a URL..."
          className="flex-1 px-4 py-2 bg-gray-800 border border-gray-700 rounded-lg focus:outline-none focus:border-emerald-500"
        />
        <button onClick={handleSearch} disabled={searching}
          className="px-4 py-2 bg-emerald-600 hover:bg-emerald-700 rounded-lg disabled:opacity-50 flex items-center gap-2">
          {searching ? <Loader2 className="animate-spin" size={16} /> : <SearchIcon size={16} />}
          Search
        </button>
      </div>

      {showResults ? (
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
          {searchResults.map((snap, i) => (
            <ComicCard key={i} snapshot={snap} onClick={() => handleClick(snap)} />
          ))}
        </div>
      ) : (
        <div>
          <h2 className="text-lg font-semibold mb-4 text-gray-400">Proposals</h2>
          {proposalLoading ? (
            <div className="flex justify-center py-12"><Loader2 className="animate-spin text-emerald-400" size={32} /></div>
          ) : (
            <div className="flex gap-4 overflow-x-auto pb-4">
              {proposals.map((snap, i) => (
                <ComicCard key={i} snapshot={snap} onClick={() => handleClick(snap)} wide />
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
}
