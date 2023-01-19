using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    public class HQueryStatistic : AnfCount
    {
        public const string AllPath = "*";

        [Required]
        [MaxLength(256)]
        public string Path { get; set; }

    }
}
