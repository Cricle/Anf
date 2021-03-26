﻿using Kw.Comic.Models;
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

        public Task<int> ClearAsync()
        {
            return dbContext.Bookshelfs.DeleteFromQueryAsync();
        }

        public async Task<SetResult<Bookshelf>> FindBookShelfAsync(string key, int? skip, int? take)
        {
            var query = dbContext.Bookshelfs.AsNoTracking();
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{key}%") || EF.Functions.Like(x.Descript, $"%{key}%"));
            }
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
