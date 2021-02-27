using GalaSoft.MvvmLight;
using Kw.Comic.Wpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf.ViewModels
{
    public class ComicViewModel : ViewModelBase
    {
        public ComicViewModel(ComicSnapshotInfo comic)
        {
            Comic = comic;
        }
        private ComicSourceInfo currentSource;

        public ComicSourceInfo CurrentSource
        {
            get { return currentSource; }
            set => Set(ref currentSource, value);
        }

        public ComicSnapshotInfo Comic { get; }
    }
}
