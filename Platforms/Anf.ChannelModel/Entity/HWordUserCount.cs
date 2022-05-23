using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    public class HWordUserCount: CountStatistic
    {
        [MaxLength(36)]
        public string IP { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        public AnfUser User { get; set; }

    }
}
