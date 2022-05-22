using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    public class AnfComicSearchRank: AnfComicRank
    {
        [Required]
        [MaxLength(250)]
        public string Content { get; set; }
    }
    public class AnfComicVisitRank : AnfComicRank
    {
        [Required]
        [MaxLength(512)]
        public string Address { get; set; }
    }

    public class AnfComicRank
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public StatisticLevels Type { get; set; }

        [Required]
        public long VisitCount { get; set; }

    }
    public class AnfComicSearch: AnfComicCount
    {
        [Required]
        [MaxLength(250)]
        public string Content { get; set; }

    }
    public class AnfComicVisit: AnfComicCount
    {
        [Required]
        [MaxLength(512)]
        public string Address { get; set; }

    }
    public class AnfComicCount
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [MaxLength(36)]
        public string IP { get; set; }

        public long? UserId { get; set; }

        [Required]
        public DateTime Time { get; set; }

        public AnfUser User { get; set; }
    }
}
