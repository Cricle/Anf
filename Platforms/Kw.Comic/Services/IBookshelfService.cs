using Kw.Comic.Models;
using Kw.Comic.Results;
using System.Threading.Tasks;

namespace Kw.Comic.Services
{
    public interface IBookshelfService
    {
        Task AddAsync(Bookshelf bookshelf);
        Task<int> ClearAsync();
        Task<SetResult<Bookshelf>> FindBookShelfAsync(string key, int? skip, int? take);
        Task<int> RemoveAsync(string address);
    }
}