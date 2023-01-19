using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web.Models
{
    public class HotSearchItem
    {
        public string Keyword { get; set; }

        public double Scope { get; set; }
    }
    public class SortedItem
    {
        public ComicEntity Entity { get; set; }

        public double Scope { get; set; }
    }
}
