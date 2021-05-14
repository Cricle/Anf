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

        [MaxLength(64)]
        public string Name { get; set; }

        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        public bool Like { get; set; }

        [ForeignKey(nameof(LinkBookshelf))]
        public ulong? LinkId { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public virtual AnfUser User { get; set; }

        public AnfBookshelf LinkBookshelf { get; set; }

        public virtual ICollection<AnfBookshelfItem> Items { get; set; }
    }
}
