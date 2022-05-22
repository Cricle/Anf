using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    public class CountStatistic
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public ulong Count { get; set; }

    }
    public class HWordUserStatistic: CountStatistic
    {
        [MaxLength(36)]
        public string IP { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        public AnfUser User { get; set; }

    }
}
