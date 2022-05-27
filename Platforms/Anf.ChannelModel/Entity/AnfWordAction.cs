using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    public abstract class AnfWordAction : AnfCount
    {
        [ForeignKey(nameof(User))]
        public long? UserId { get; set; }

        [Required]
        [ForeignKey(nameof(Word))]
        public ulong WordId { get; set; }

        [Required]
        public bool Enable { get; set; }

        public DateTime? UpdateTime { get; set; }

        public AnfWord Word { get; set; }

        public AnfUser User { get; set; }
    }
}
