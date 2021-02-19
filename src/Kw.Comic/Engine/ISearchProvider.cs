using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Engine
{
    public interface ISearchProvider
    {
        Task<SearchComicResult> SearchAsync(string keywork,int skip,int take);
    }
}
