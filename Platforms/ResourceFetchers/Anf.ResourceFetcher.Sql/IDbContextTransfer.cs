using Anf.ChannelModel.Entity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public interface IDbContextTransfer
    {
        DbSet<KvComicChapter> GetComicChapterSet();

        DbSet<KvComicEntity> GetComicEntitySet();

        DbContext Context { get; }
    }
}
