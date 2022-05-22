using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Anf.ChannelModel.Entity
{
    public class HWordReadStatistic : CountStatistic
    {
        [MaxLength(36)]
        public string IP { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        [Required]
        [ForeignKey(nameof(Word))]
        public long WordId { get; set; }

        public HWord Word { get; set; }

        public AnfUser User { get; set; }

    }
}
