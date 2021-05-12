using Anf.ChannelModel.Mongo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Anf.ChannelModel.Entity
{
    public class KvComicEntity: AnfComicEntityInfoOnly
    {
        [Key]
        public ulong Id { get; set; }

        public virtual ICollection<KvComicChapter> Chapters { get; set; }
    }
}
