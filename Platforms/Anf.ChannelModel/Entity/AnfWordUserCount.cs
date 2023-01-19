using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    public class AnfWordUserCount: AnfCount
    {
        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        public AnfUser User { get; set; }

    }
    public class AnfWordUserStatistic : AnfStatistic
    {
        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        public AnfUser User { get; set; }

    }
}
