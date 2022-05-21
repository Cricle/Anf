using Anf.ChannelModel.Entity;
using Anf.ResourceFetcher.Fetchers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.WebService
{
    internal class AnfDbContextTransfer : IDbContextTransfer
    {
        private readonly AnfDbContext dbContext;

        public AnfDbContextTransfer(AnfDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public DbContext Context => dbContext;

        public DbSet<KvComicChapter> GetComicChapterSet()
        {
            return dbContext.ComicChapters;
        }

        public DbSet<KvComicEntity> GetComicEntitySet()
        {
            return dbContext.ComicEntities;
        }
    }
}
