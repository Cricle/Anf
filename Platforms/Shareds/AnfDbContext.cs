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

        public DbSet<AnfBookshelf> Bookshelves => Set<AnfBookshelf>();

        public DbSet<AnfBookshelfItem> BookshelfItems => Set<AnfBookshelfItem>();

        public DbSet<KvComicEntity> ComicEntities => Set<KvComicEntity>();

        public DbSet<KvComicChapter> ComicChapters => Set<KvComicChapter>();

        public DbSet<AnfSearchStatistic> SearchRanks => Set<AnfSearchStatistic>();

        public DbSet<AnfVisitStatistic> VisitRanks => Set<AnfVisitStatistic>();

        public DbSet<AnfVisitCount> Visits => Set<AnfVisitCount>();

        public DbSet<AnfSearchCount> Searchs => Set<AnfSearchCount>();

        public DbSet<AnfApp> Apps => Set<AnfApp>();

        public DbSet<AnfWord> Words => Set<AnfWord>();

        public DbSet<AnfWordLike> WordLikes => Set<AnfWordLike>();

        public DbSet<AnfWordVisit> WordVisits => Set<AnfWordVisit>();

        public DbSet<AnfWordReadCount> WordReadCount => Set<AnfWordReadCount>();

        public DbSet<AnfWordUpdateCount> WordUpdateCount => Set<AnfWordUpdateCount>();

        public DbSet<AnfWordUserCount> WordUserCount => Set<AnfWordUserCount>();

        public DbSet<AnfQueryCount> QueryCount => Set<AnfQueryCount>();


        public DbSet<AnfWorkReadStatistic> WordReadStatistics => Set<AnfWorkReadStatistic>();

        public DbSet<AnfWordUpdateStatistic> WordUpdateStatistics => Set<AnfWordUpdateStatistic>();

        public DbSet<AnfWordUserStatistic> WordUserStatistics => Set<AnfWordUserStatistic>();

        public DbSet<AnfQueryStatistic> QueryStatistics => Set<AnfQueryStatistic>();


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
            builder.Entity<AnfSearchStatistic>(x => 
            {
                x.HasIndex(x => x.Content);
            }); 
            builder.Entity<AnfSearchStatistic>(x =>
            {
                x.HasIndex(x => x.Content);
            });
            builder.Entity<AnfVisitCount>(x =>
            {
                x.HasIndex(x => x.Address);
                x.HasIndex(x => x.Time);
            });
            builder.Entity<AnfSearchCount>(x =>
            {
                x.HasIndex(x => x.Content);
                x.HasIndex(x => x.Time);
            });


            builder.Entity<AnfApp>(b =>
            {
            });
            builder.Entity<AnfWord>(b =>
            {
                b.HasIndex(x => x.Length);
                b.HasIndex(x => x.Type);
#if NET6_0_OR_GREATER
                b.Property(x => x.CreatorId)
                    .IsSparse(true);
#endif
            });

            builder.Entity<AnfWordLike>(b =>
            {
                b.HasIndex(x => new { x.WordId, x.UserId });
                b.HasIndex(x => new { x.WordId, x.IP });

                b.HasOne(x => x.Word)
                    .WithMany(x => x.Likes)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            builder.Entity<AnfWordVisit>(b =>
            {
                b.HasIndex(x => new { x.WordId, x.UserId });
                b.HasIndex(x => new { x.WordId, x.IP });

                b.HasOne(x => x.Word)
                    .WithMany(x => x.Visits)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            builder.Entity<AnfWordReadCount>(b =>
            {
                b.HasIndex(x => x.WordId);
                b.HasIndex(x => x.Time);
            });
            builder.Entity<AnfWordUpdateCount >(b =>
            {
                b.HasIndex(x => x.Time);
            });
            builder.Entity<AnfWordUserCount>(b =>
            {
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.Time);
            });
            builder.Entity<AnfQueryCount>(b =>
            {
                b.HasIndex(x => x.Path);
                b.HasIndex(x => x.Time);
            });
        }
    }
}
