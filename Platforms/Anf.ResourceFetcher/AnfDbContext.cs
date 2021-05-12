using Anf.ChannelModel.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher
{
    public class AnfDbContext : IdentityDbContext<AnfUser, AnfRole, long>
    {
        public AnfDbContext(DbContextOptions options) : base(options)
        {

        }

        protected AnfDbContext()
        {
        }

        public DbSet<AnfBookshelf> Bookshelves { get; set; }

        public DbSet<AnfBookshelfItem> BookshelfItems { get; set; }

        public DbSet<KvComicEntity> ComicEntities { get; set; }

        public DbSet<KvComicChapter> ComicChapters { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<KvComicEntity>(x =>
            {
                x.HasIndex(y => y.ComicUrl).IsUnique(true);
                x.HasIndex(y => y.UpdateTime);
            });
            builder.Entity<KvComicChapter>(x =>
            {
                x.HasKey(y => new { y.TargetUrl, y.EnitityId });
                x.HasIndex(y => y.UpdateTime);
            });
        }
    }
}
