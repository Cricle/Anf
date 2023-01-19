using Anf.ChannelModel.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Core.Models
{
    public class RangeVisitKey
    {
        public int Count { get; set; }

        public override string ToString()
        {
            return Count.ToString();
        }
    }
    public class RangeVisitEntity
    {
        public AnfComicEntityScoreTruck[] EntityTrucks { get; set; }

        public long Size { get; set; }

        public int HitCount { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
