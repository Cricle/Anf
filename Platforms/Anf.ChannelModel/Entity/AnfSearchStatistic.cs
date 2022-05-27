using System.ComponentModel.DataAnnotations;

namespace Anf.ChannelModel.Entity
{
    public class AnfSearchStatistic: AnfStatistic
    {
        [Required]
        [MaxLength(250)]
        public string Content { get; set; }
    }
}
