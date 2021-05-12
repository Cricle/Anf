using Anf.ChannelModel.Mongo;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    public class KvComicChapter : WithPageChapterInfoOnly
    {
        [DataType(DataType.Text)]
        public string Pages { get; set; }

        [Required]
        [ForeignKey(nameof(Entity))]
        public ulong EnitityId { get; set; }

        public virtual KvComicEntity Entity { get; set; }
    }
}
