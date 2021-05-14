using Anf.ChannelModel.Entity;
using Microsoft.EntityFrameworkCore;

namespace Anf.ResourceFetcher.Fetchers
{
    public interface IDbContextTransfer
    {
        DbSet<KvComicChapter> GetComicChapterSet();

        DbSet<KvComicEntity> GetComicEntitySet();
    }
}
