using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Anf.ChannelModel.Entity
{
    public class AnfHourComicRank: AnfComicRank
    {

    }
    public class AnfDayComicRank : AnfComicRank
    {

    }
    public class AnfMonthComicRank : AnfComicRank
    {

    }
    public class AnfComicRank
    {
        [Required]
        public DateTime Time { get; set; }

        [Required]
        public int No { get; set; }

        [Required]
        public double VisitCount { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
