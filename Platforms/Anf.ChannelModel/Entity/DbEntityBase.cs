using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anf.ChannelModel.Entity
{
    [NotMapped]
    public abstract class DbEntityBase
    {
        public DateTime CreateTime { get; set; }
    }
}
