using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Anf.ChannelModel.Entity
{
    public class AnfWorkReadStatistic:AnfStatistic
    {
        [Required]
        [ForeignKey(nameof(Word))]
        public ulong WordId { get; set; }

        public AnfWord Word { get; set; }
    }
    public class AnfWordReadCount : AnfCount
    {

        [Required]
        [ForeignKey(nameof(Word))]
        public ulong WordId { get; set; }

        public AnfWord Word { get; set; }

    }
}
