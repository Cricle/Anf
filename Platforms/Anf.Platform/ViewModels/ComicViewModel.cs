using Anf.Models;
using Anf.Platform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anf.Platform.ViewModels
{
    public class ComicViewModel : StorableComicSnapshotInfo
    {
        public ComicViewModel(ComicSnapshot snapshot)
            : base(snapshot)
        {
            ComicStoreService = AppEngine.GetRequiredService<ComicStoreService>();
        }

        public ComicStoreService ComicStoreService { get; }

    }
}
