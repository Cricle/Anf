using Anf.ChannelModel.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.WebService
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

        public DbSet<AnfDayComicRank> DayRanks { get; set; }
        
        public DbSet<AnfHourComicRank> HourRanks { get; set; }

        public DbSet<AnfMonthComicRank> MonthRanks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AnfBookshelf>(x =>
            {
                x.Property(y => y.Id).ValueGeneratedOnAdd();
            });
            builder.Entity<AnfBookshelfItem>(x =>
            {
                x.HasKey(y => new { y.Address, y.BookshelfId });
                x.HasOne(y => y.User)
                    .WithMany(y => y.BookshelfItems)
                    .HasForeignKey(nameof(AnfBookshelfItem.UserId))
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            builder.Entity<KvComicEntity>(x =>
            {
                x.HasIndex(y => y.ComicUrl).IsUnique(true);
                x.HasIndex(y => y.UpdateTime);
                x.Property(y => y.Id).ValueGeneratedOnAdd();
            });
            builder.Entity<KvComicChapter>(x =>
            {
                x.HasKey(y => new { y.TargetUrl, y.EnitityId });
                x.HasIndex(y => y.UpdateTime);
            });
            InitRank<AnfHourComicRank>();
            InitRank<AnfDayComicRank>();
            InitRank<AnfMonthComicRank>();
            void InitRank<T>()
                where T:AnfComicRank
            {
                var entityBuilder = builder.Entity<T>();
                entityBuilder.HasKey(nameof(AnfComicRank.Time), nameof(AnfComicRank.No));
                entityBuilder.HasIndex(nameof(AnfComicRank.Address));
            }
        }
    }
}
