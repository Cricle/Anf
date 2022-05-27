using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    public class AnfVisitCount: AnfCount
    {
        [Required]
        [MaxLength(512)]
        public string Address { get; set; }

    }
}
