import { Routes, Route, useNavigate, useLocation } from "react-router-dom";
import { Search, BookOpen, Settings, Home } from "lucide-react";
import HomePage from "./pages/HomePage";
import ComicDetail from "./pages/ComicDetail";
import Reader from "./pages/Reader";
import Bookshelf from "./pages/Bookshelf";

export default function App() {
  const navigate = useNavigate();
  const location = useLocation();
  const isReader = location.pathname.startsWith("/read");

  return (
    <div className="flex flex-col h-screen">
      {!isReader && (
        <nav className="flex items-center gap-4 px-4 py-2 bg-gray-900 border-b border-gray-800">
          <button onClick={() => navigate("/")} className="flex items-center gap-2 text-lg font-bold text-emerald-400">
            <Home size={20} /> Anf
          </button>
          <div className="flex-1" />
          <button onClick={() => navigate("/")} className="flex items-center gap-1 text-sm hover:text-emerald-400"><Search size={16} /> Search</button>
          <button onClick={() => navigate("/shelf")} className="flex items-center gap-1 text-sm hover:text-emerald-400"><BookOpen size={16} /> Bookshelf</button>
        </nav>
      )}
      <main className="flex-1 overflow-auto">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/comic" element={<ComicDetail />} />
          <Route path="/read" element={<Reader />} />
          <Route path="/shelf" element={<Bookshelf />} />
        </Routes>
      </main>
    </div>
  );
}
