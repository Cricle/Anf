using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    public class AnfStatistic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [Required]
        public StatisticLevels Type { get; set; }

        [Required]
        public long Count { get; set; }

        [Required]
        public DateTime Time { get; set; }
    }
}
