using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.ChannelModel.Entity
{
    public class AnfBookshelf
    {
        [Key]
        public ulong Id { get; set; }

        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        public bool Like { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public virtual AnfUser User { get; set; }

        public virtual ICollection<AnfBookshelfItem> Items { get; set; }
    }
}
