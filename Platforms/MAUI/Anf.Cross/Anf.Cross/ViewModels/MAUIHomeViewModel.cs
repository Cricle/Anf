using Anf.Cross.Settings;
using Anf.Models;
using Anf.Networks;
using Anf.Platform.Models.Impl;
using Anf.ViewModels;
using GalaSoft.MvvmLight;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Cross.ViewModels
{
    public class MAUIHomeViewModel : HomeViewModel<WithImageStorableComicSourceInfo<Stream, ImageSource>, ImageSource>
    {
        public AnfSettings AnfSettings { get; }
        private readonly INetworkAdapter networkAdapter;

        public MAUIHomeViewModel()
        {
            AnfSettings = AppEngine.GetRequiredService<AnfSettings>();
            this.networkAdapter = AppEngine.GetRequiredService<INetworkAdapter>();
        }

        protected override ComicSnapshotInfo<WithImageStorableComicSourceInfo<Stream, ImageSource>> CreateSnapshotInfo(ComicSnapshot info)
        {
            return new WithImageComicSnapshotInfo<Stream, ImageSource>(info, networkAdapter);
        }
    }
}
