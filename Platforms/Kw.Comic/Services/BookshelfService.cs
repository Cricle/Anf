using Kw.Comic.Models;
using Kw.Comic.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Services
{
    internal class BookshelfService : IBookshelfService
    {
        private readonly ComicDbContext dbContext;

        public BookshelfService(ComicDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task AddAsync(Bookshelf bookshelf)
        {
            return dbContext.Bookshelfs.SingleInsertAsync(bookshelf);
        }
        public Task<int> RemoveAsync(string address)
        {
            return dbContext.Bookshelfs.Where(x => x.ComicUrl == address)
                .Take(1)
                .DeleteFromQueryAsync();
        }
        public Task<int> SetChapterAsync(string address,int chapterIndex,int? pageIndex)
        {
            return dbContext.Bookshelfs.Where(x => x.ComicUrl == address)
                .UpdateFromQueryAsync(x => new Bookshelf
                {
                    ReadChapter = chapterIndex,
                    ReadPage = pageIndex ?? x.ReadPage
                });
        }
        public Task<int> ClearAsync()
        {
            return dbContext.Bookshelfs.DeleteFromQueryAsync();
        }

        public async Task<SetResult<Bookshelf>> FindBookShelfAsync(int? skip, int? take)
        {
            var query = dbContext.Bookshelfs.AsNoTracking();
            var count = await query.LongCountAsync();
            if (skip != null)
            {
                query = query.Skip(skip.Value);
            }
            if (take != null)
            {
                query = query.Take(take.Value);
            }
            var datas = await query.ToArrayAsync();
            return new SetResult<Bookshelf>
            {
                Take = take,
                Skip = skip,
                Total = count,
                Datas = datas
            };
        }
    }
}
