using Anf.ChannelModel.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.WebService
{
    public class ComicRankSaver
    {
        private readonly ComicRankService comicRankService;
        private readonly AnfDbContext dbContext;
        private readonly IOptions<ComicRankOptions> rankOptions;

        public ComicRankSaver(ComicRankService comicRankService, AnfDbContext dbContext, IOptions<ComicRankOptions> rankOptions)
        {
            this.comicRankService = comicRankService;
            this.dbContext = dbContext;
            this.rankOptions = rankOptions;
        }

        public Task<AnfComicRank[]> SaveAsync(RankLevels level)
        {
            if (level == RankLevels.Hour)
            {
                return SaveAsync(RankLevels.Hour, dbContext.HourRanks);
            }
            else if (level == RankLevels.Day)
            {
                return SaveAsync(RankLevels.Day, dbContext.DayRanks);
            }
            else if (level == RankLevels.Month)
            {
                return SaveAsync(RankLevels.Month, dbContext.MonthRanks);
            }
            return Task.FromResult<AnfComicRank[]>(null);
        }
        public async Task<AnfComicRank[]> SaveAsync<T>(RankLevels level,DbSet<T> rankSet)
            where T: AnfComicRank,new()
        {
            var datas = await comicRankService.RangeAsync(rankOptions.Value.SaveRankCount);
            if (datas.Length == 0)
            {
                return Array.Empty<AnfComicRank>();
            }
            var time = DateTime.Now;
            if (level== RankLevels.Hour)
            {
                time = new DateTime(time.Year, time.Month, time.Day, time.Hour,0,0);
            }
            else if (level == RankLevels.Day)
            {
                time = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0);
            }
            else if (level == RankLevels.Month)
            {
                time = new DateTime(time.Year, time.Month, 1, 0, 0, 0);
            }
            else
            {
                throw new NotSupportedException(level.ToString());
            }
            var i = 1;
            var entities = datas.Select(x => new T
            {
                Address = x.Element.ToString(),
                No = i++,
                Time = time,
                VisitCount = x.Score
            }).ToArray();
            await rankSet.BulkInsertAsync(entities);
            return entities;
        }
    }
}
