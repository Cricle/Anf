using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Uwp.Models
{
    public class ComicMenuItem : ObservableObject
    {
        private object icon;
        private string text;

        public ComicMenuItem(object icon, string text, Type pageType)
        {
            Icon = icon;
            Text = text;
            PageType = pageType;
        }

        public string Text
        {
            get { return text; }
            set => Set(ref text, value);
        }

        public object Icon
        {
            get { return icon; }
            set => Set(ref icon, value);
        }

        public Type PageType { get; }
    }
}
