using Anf.ChannelModel.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

        public DbSet<AnfComicSearchRank> SearchRanks { get; set; }

        public DbSet<AnfComicVisitRank> VisitRanks { get; set; }

        public DbSet<AnfComicVisit> Visits { get; set; }

        public DbSet<AnfComicSearch> Searchs { get; set; }

        public DbSet<AnfApp> Apps { get; set; }

        public DbSet<HWord> Words { get; set; }

        public DbSet<HWordLike> WordLikes { get; set; }

        public DbSet<HWordVisit> WordVisits { get; set; }

        public DbSet<HWordReadCount> WordReadStatistics { get; set; }

        public DbSet<HWordUpdateCount> WordUpdateStatistics { get; set; }

        public DbSet<HWordUserCount> WordUserStatistics { get; set; }

        public DbSet<HQueryStatistic> QueryStatistics { get; set; }


#if NET6_0_OR_GREATER&&!COMPILE_EF_TIME
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseModel(AnfDbContextModel.Instance);
        }
#endif

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
            builder.Entity<AnfComicSearchRank>(x => 
            {
                x.HasIndex(x => x.Content);
            }); 
            builder.Entity<AnfComicSearchRank>(x =>
            {
                x.HasIndex(x => x.Content);
            });
            builder.Entity<AnfComicVisit>(x =>
            {
                x.HasIndex(x => x.Address);
                x.HasIndex(x => x.Time);
            });
            builder.Entity<AnfComicSearch>(x =>
            {
                x.HasIndex(x => x.Content);
                x.HasIndex(x => x.Time);
            });


            builder.Entity<AnfApp>(b =>
            {
            });
            builder.Entity<HWord>(b =>
            {
                b.HasIndex(x => x.Length);
                b.HasIndex(x => x.Type);
            });

            builder.Entity<HWordLike>(b =>
            {
                b.HasIndex(x => new { x.WordId, x.UserId });
                b.HasIndex(x => new { x.WordId, x.Ip });

                b.HasOne(x => x.Word)
                    .WithMany(x => x.Likes)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            builder.Entity<HWordVisit>(b =>
            {
                b.HasIndex(x => new { x.WordId, x.UserId });
                b.HasIndex(x => new { x.WordId, x.Ip });

                b.HasOne(x => x.Word)
                    .WithMany(x => x.Visits)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            builder.Entity<HWordReadCount>(b =>
            {
                b.HasIndex(x => x.WordId);
                b.HasIndex(x => x.Time);
            });
            builder.Entity<HWordUpdateCount >(b =>
            {
                b.HasIndex(x => x.Time);
            });
            builder.Entity<HWordUserCount>(b =>
            {
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.Time);
            });
            builder.Entity<HQueryStatistic>(b =>
            {
                b.HasIndex(x => x.Path);
                b.HasIndex(x => x.Time);
            });
        }
    }
}
