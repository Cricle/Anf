using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    public abstract class HWordConnect
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [MaxLength(36)]
        public string Ip { get; set; }

        [ForeignKey(nameof(User))]
        public long? UserId { get; set; }

        [Required]
        [ForeignKey(nameof(Word))]
        public ulong WordId { get; set; }

        [Required]
        public bool Enable { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public HWord Word { get; set; }

        public AnfUser User { get; set; }
    }
}
