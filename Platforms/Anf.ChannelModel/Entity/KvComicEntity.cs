using Anf.ChannelModel.Mongo;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Anf.ChannelModel.Entity
{
    public class KvComicEntity : AnfComicEntityInfoOnly
    {
        [Key]
        public long Id { get; set; }

        public virtual ICollection<KvComicChapter> Chapters { get; set; }
    }
}
