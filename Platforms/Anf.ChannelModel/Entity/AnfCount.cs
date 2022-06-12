using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    public class AnfCount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [MaxLength(36)]
        public string IP { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public long? UserId { get; set; }

        public AnfUser User { get; set; }
    }
}
