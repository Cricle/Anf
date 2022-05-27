using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    public class AnfQueryCount : AnfCount
    {
        [Required]
        [MaxLength(256)]
        public string Path { get; set; }

    }
    public class AnfQueryStatistic : AnfStatistic
    {
        [Required]
        [MaxLength(256)]
        public string Path { get; set; }

    }
}
