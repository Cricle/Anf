using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Anf.Phone.Models
{
    public class MainPageMenuItem
    {
        public string Icon { get; set; }

        public string Name { get; set; }

        public Func<Page> PageCreator { get; set; }
    }
}
