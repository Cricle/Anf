using Avalonia.Controls;
using Avalonia.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using Kw.Comic.Engine;
using Kw.Comic.Engine.Easy;
using Kw.Comic.Engine.Easy.Visiting;
using Kw.Comic.Models;
using Kw.Comic.ViewModels;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon.ViewModels
{
    public class AvalonVisitingViewModel : VisitingViewModel<Bitmap, Bitmap>
    {
        public static async Task<AvalonVisitingViewModel> CreateAsync(string address)
        {
            var vm = new AvalonVisitingViewModel();
            var ok = await vm.Visiting.LoadAsync(address);
            if (ok)
            {
                await vm.Init();
            }
            return vm;
        }
        public AvalonVisitingViewModel()
            : base(null)
        {
            SaveImageCommand = new RelayCommand<ComicPageInfo<Bitmap>>(SaveImage);
        }
        public AvalonVisitingViewModel(IComicVisiting<Bitmap> visiting = null)
            : base(visiting)
        {
            SaveImageCommand = new RelayCommand<ComicPageInfo<Bitmap>>(SaveImage);
        }

        public AvalonVisitingViewModel(IComicVisiting<Bitmap> visiting, HttpClient httpClient, RecyclableMemoryStreamManager recyclableMemoryStreamManager, IStreamImageConverter<Bitmap> streamImageConverter)
            : base(visiting, httpClient, recyclableMemoryStreamManager, streamImageConverter)
        {
            SaveImageCommand = new RelayCommand<ComicPageInfo<Bitmap>>(SaveImage);
        }
        private bool chapterSelectorOpen;
        public ComicChapter TrulyCurrentComicChapter
        {
            get => this.CurrentChapter;
            set
            {
                _ = GoChapterAsync(value);
                RaisePropertyChanged();
            }
        }
        public bool ChapterSelectorOpen
        {
            get { return chapterSelectorOpen; }
            set => Set(ref chapterSelectorOpen, value);
        }
        public RelayCommand<ComicPageInfo<Bitmap>> SaveImageCommand { get; }

        public async void SaveImage(ComicPageInfo<Bitmap> info)
        {
            var name = $"{PathHelper.EnsureName(Name)}-{PathHelper.EnsureName(CurrentChapter.Title)}-{info.Index}.jpg";
            await info.Resource.PickSaveAsync(name);
        }
    }
}
