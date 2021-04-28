using Anf.Models;
using Anf.ViewModels;
using Anf.WebClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.WebClient.ViewModels
{
    public class WebHomeViewModel : HomeViewModel<ComicSourceInfo, Stream>
    {
        protected override ComicSnapshotInfo<ComicSourceInfo> CreateSnapshotInfo(ComicSnapshot info)
        {
            return new WebComicSnapshotInfo(info);
        }
    }
}
