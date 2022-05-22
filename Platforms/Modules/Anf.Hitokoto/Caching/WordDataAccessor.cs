using Anf.ChannelModel;
using Anf.WebService;
using Ao.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Hitokoto.Caching
{
    public class WordDataAccessor : DataAccesstorBase<ulong, WordResponse>
    {
        private static readonly Func<AnfDbContext, ulong, Task<WordResponse>> findWord =
            EF.CompileAsyncQuery<AnfDbContext, ulong, WordResponse>((a, b) =>
                  a.Words.AsNoTracking()
                      .Where(x => x.Id == b)
                      .Select(x => new WordResponse
                      {
                          Id = x.Id,
                          CommitType = x.CommitType,
                          CreateTime = x.CreateTime,
                          From = x.From,
                          Length = x.Length,
                          LikeCount = x.LikeCount,
                          Text = x.Text,
                          Type = x.Type,
                          VisitCount = x.VisitCount
                      })
                      .FirstOrDefault());

        public WordDataAccessor(AnfDbContext dbContext, IOptions<WordCacheOptions> options)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public AnfDbContext DbContext { get; }

        public IOptions<WordCacheOptions> Options { get; }

        public override Task<WordResponse> FindAsync(ulong identity)
        {
            if (identity <= 0)
            {
                return Task.FromResult<WordResponse>(null);
            }
            return findWord(DbContext, identity);
        }

        public override TimeSpan? GetCacheTime(ulong identity, WordResponse entity)
        {
            return Options.Value.CacheTime;
        }
    }
}
