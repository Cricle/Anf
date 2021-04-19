using Avalonia.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using Anf.Easy;
using Anf.Easy.Visiting;
using Anf.Models;
using Anf.ViewModels;
using Microsoft.IO;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls.PanAndZoom;
using Anf.Avalon.Services;
using Anf.Avalon.Models;

namespace Anf.Avalon.ViewModels
{
    public class AvalonVisitingViewModel : VisitingViewModel<Bitmap, Bitmap,AvalonComicStoreBox>
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
            AvalonInit();
        }
        public AvalonVisitingViewModel(IComicVisiting<Bitmap> visiting = null)
            : base(visiting)
        {
            AvalonInit();
        }

        public AvalonVisitingViewModel(IComicVisiting<Bitmap> visiting, HttpClient httpClient, RecyclableMemoryStreamManager recyclableMemoryStreamManager, IStreamImageConverter<Bitmap> streamImageConverter)
            : base(visiting, httpClient, recyclableMemoryStreamManager, streamImageConverter)
        {
            AvalonInit();
        }
        private bool chapterSelectorOpen;
        private StretchMode stretchMode;
        private double zoomSpeed;
        private bool enableGesture;
        private bool enableConstrains;
        private bool enableZoom;
        private bool transverse;
        private double minWidth;
        private double minHeight;
        private ComicPageInfo<Bitmap> selectedResource;
        private bool statusShow;

        public bool StatusShow
        {
            get { return statusShow; }
            set => Set(ref statusShow, value);
        }

        public ComicPageInfo<Bitmap> SelectedResource
        {
            get { return selectedResource; }
            private set => Set(ref selectedResource, value);
        }

        public double MinHeight
        {
            get { return minHeight; }
            set => Set(ref minHeight, value);
        }

        public double MinWidth
        {
            get { return minWidth; }
            set => Set(ref minWidth, value);
        }

        public bool Transverse
        {
            get { return transverse; }
            set
            {
                var origin = transverse;
                Set(ref transverse, value);
                if (origin != value)
                {
                    TransverseChanged?.Invoke(this, value);
                }
            }
        }

        public bool EnableZoom
        {
            get { return enableZoom; }
            set => Set(ref enableZoom, value);
        }

        public bool EnableConstrains
        {
            get { return enableConstrains; }
            set => Set(ref enableConstrains, value);
        }

        public bool EnableGesture
        {
            get { return enableGesture; }
            set => Set(ref enableGesture, value);
        }

        public double ZoomSpeed
        {
            get { return zoomSpeed; }
            set => Set(ref zoomSpeed, value);
        }

        public StretchMode StretchMode
        {
            get { return stretchMode; }
            set => Set(ref stretchMode, value);
        }

        public ComicChapter TrulyCurrentComicChapter
        {
            get => this.CurrentChapter;
            set
            {
                _ = AvalonGoChapterAsync(value);
            }
        }
        private async Task AvalonGoChapterAsync(ComicChapter chapter)
        {
            try
            {
                await GoChapterAsync(chapter);
                RaisePropertyChanged(nameof(TrulyCurrentComicChapter));
            }
            catch (Exception ex)
            {
                ExceptionService.Exception = ex;
            }
        }
        public bool ChapterSelectorOpen
        {
            get { return chapterSelectorOpen; }
            set => Set(ref chapterSelectorOpen, value);
        }

        public StretchMode[] StretchModes { get; protected set; }
        internal TitleService TitleService { get; set; }
        public event Action<AvalonVisitingViewModel, bool> TransverseChanged;
        internal ExceptionService ExceptionService { get; set; }
        public RelayCommand<ComicPageInfo<Bitmap>> SaveImageCommand { get; protected set; }
        private void AvalonInit()
        {
            MinWidth = 200;
            MinHeight = 400;
            ZoomSpeed = 2;
            StretchMode = StretchMode.None;
            StretchModes = ZoomBorder.StretchModes;
            SaveImageCommand = new RelayCommand<ComicPageInfo<Bitmap>>(SaveImage);
            PageCursorMoved += AvalonVisitingViewModel_PageCursorMoved;
            TitleService = AppEngine.GetRequiredService<TitleService>();
            ExceptionService = AppEngine.GetRequiredService<ExceptionService>();
        }
        protected override void OnInitDone()
        {
            TitleService.Title = $"Anf - {ComicEntity.Name}";
        }

        private void AvalonVisitingViewModel_PageCursorMoved(IDataCursor<IComicVisitPage<Bitmap>> arg1, int arg2)
        {
            var res = this.Resources;
            if (arg2 >= 0 && arg2 < res.Count)
            {
                SelectedResource = res[arg2];
            }
        }

        public async void SaveImage(ComicPageInfo<Bitmap> info)
        {
            try
            {
                var name = $"{PathHelper.EnsureName(Name)}-{PathHelper.EnsureName(CurrentChapter.Title)}-{info.Index}.jpg";
                await info.Resource.PickSaveAsync(name);
            }
            catch (Exception ex)
            {
                ExceptionService.Exception = ex;
            }
        }
        protected async override void OnCurrentChaterCursorChanged(IDataCursor<IComicChapterManager<Bitmap>> cursor)
        {
            try
            {
                SelectedResource = null;
                await LoadAllAsync();
            }
            catch (Exception ex)
            {
                ExceptionService.Exception = ex;
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            TitleService.Title = string.Empty;
        }
    }
}
