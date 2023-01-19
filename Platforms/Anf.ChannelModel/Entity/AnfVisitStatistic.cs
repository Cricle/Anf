using System.ComponentModel.DataAnnotations;

namespace Anf.ChannelModel.Entity
{
    public class AnfVisitStatistic : AnfStatistic
    {
        [Required]
        [MaxLength(512)]
        public string Address { get; set; }
    }
}
