using System.Text;
using System.Threading.Tasks;

namespace Anf
{
    public interface ISearchProvider
    {
        Task<SearchComicResult> SearchAsync(string keywork,int skip,int take);
    }
}
