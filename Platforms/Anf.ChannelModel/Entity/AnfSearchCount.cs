using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    public class AnfSearchCount: AnfCount
    {
        [Required]
        [MaxLength(250)]
        public string Content { get; set; }

        [ForeignKey(nameof(User))]
        public long? UserId { get; set; }

        public AnfUser User { get; set; }
    }
}
