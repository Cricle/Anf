using Anf.Easy;
using Anf.Easy.Visiting;
using Anf.Models;
using Anf.Platform;
using Anf.Platform.Models.Impl;
using Anf.Platform.Services;
using Anf.Platform.Settings;
using Anf.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Anf.ViewModels
{
    internal class UnoVisitingViewModel : StoreBoxVisitingViewModel<ImageSource, ImageSource, WithImageComicStoreBox<ImageSource, ImageSource>>
    {
        public static async Task<UnoVisitingViewModel> CreateAsync(string address, bool usingStore = false)
        {
            var vm = new UnoVisitingViewModel(x =>
            {
                var visiting = x.GetRequiredService<StoreComicVisiting<ImageSource>>();
                visiting.UseStore = usingStore;
                return visiting;
            });
            var ok = await vm.Visiting.LoadAsync(address);
            if (ok)
            {
                await vm.Init();
            }
            return vm;
        }
        public UnoVisitingViewModel(Func<IServiceProvider, IComicVisiting<ImageSource>> visiting = null)
            : base(visiting)
        {
            AvalonInit();
        }

        public UnoVisitingViewModel(IComicVisiting<ImageSource> visiting, HttpClient httpClient, RecyclableMemoryStreamManager recyclableMemoryStreamManager, IStreamImageConverter<ImageSource> streamImageConverter, IObservableCollectionFactory observableCollectionFactory)
            : base(visiting, httpClient, recyclableMemoryStreamManager, streamImageConverter, observableCollectionFactory)
        {
            AvalonInit();
        }
        //private StretchMode stretchMode;
        private double zoomSpeed;
        private bool enableGesture;
        private bool enableConstrains;
        private bool enableZoom;
        private bool transverse;
        private double minWidth;
        private double minHeight;
        private ComicPageInfo<ImageSource> selectedResource;
        private bool statusShow;
        private bool leftPaneOpen;
        private IDisposable readingSubscriber;

        public ReadingSettings ReadingSettings { get; private set; }
        //public StretchMode[] StretchModes { get; private set; }
        public RelayCommand OpenPaneCommand { get; private set; }
        public RelayCommand ClosePaneCommand { get; private set; }
        public RelayCommand SaveLogoCommand { get; private set; }
        public RelayCommand<ComicPageInfo<ImageSource>> SaveImageCommand { get; private set; }

        //internal TitleService TitleService { get; set; }
        internal ExceptionService ExceptionService { get; set; }

        public event Action<UnoVisitingViewModel, bool> TransverseChanged;

        public bool LeftPaneOpen
        {
            get => leftPaneOpen;
            set => SetProperty(ref leftPaneOpen, value);
        }

        public bool StatusShow
        {
            get => statusShow;
            set => SetProperty(ref statusShow, value);
        }

        public ComicPageInfo<ImageSource> SelectedResource
        {
            get => selectedResource;
            private set => SetProperty(ref selectedResource, value);
        }

        public double MinHeight
        {
            get => minHeight;
            set => SetProperty(ref minHeight, value);
        }

        public double MinWidth
        {
            get => minWidth;
            set => SetProperty(ref minWidth, value);
        }

        public bool Transverse
        {
            get => transverse;
            set
            {
                var origin = transverse;
                SetProperty(ref transverse, value);
                if (origin != value)
                {
                    TransverseChanged?.Invoke(this, value);
                }
            }
        }

        public bool EnableZoom
        {
            get => enableZoom;
            set => SetProperty(ref enableZoom, value);
        }

        public bool EnableConstrains
        {
            get => enableConstrains;
            set => SetProperty(ref enableConstrains, value);
        }

        public bool EnableGesture
        {
            get => enableGesture;
            set => SetProperty(ref enableGesture, value);
        }

        public double ZoomSpeed
        {
            get => zoomSpeed;
            set => SetProperty(ref zoomSpeed, value);
        }

        //public StretchMode StretchMode
        //{
        //    get => stretchMode;
        //    set => SetProperty(ref stretchMode, value);
        //}

        public ComicChapter TrulyCurrentComicChapter
        {
            get => CurrentChapter;
            set => _ = AvalonGoChapterAsync(value);
        }
        public void OpenPane()
        {
            LeftPaneOpen = true;
        }
        public void ClosePane()
        {
            LeftPaneOpen = false;
        }
        private async Task AvalonGoChapterAsync(ComicChapter chapter)
        {
            try
            {
                await GoChapterAsync(chapter);
            }
            catch (Exception ex)
            {
                ExceptionService.Exception = ex;
            }
        }
        private void AvalonInit()
        {
            MinWidth = 200;
            MinHeight = 400;
            ZoomSpeed = 1;
            //StretchMode = StretchMode.UniformToFill;
            //StretchModes = ZoomBorder.StretchModes;
            SaveImageCommand = new RelayCommand<ComicPageInfo<ImageSource>>(SaveImage);
            SaveLogoCommand = new RelayCommand(SaveLogo);
            OpenPaneCommand = new RelayCommand(OpenPane);
            ClosePaneCommand = new RelayCommand(ClosePane);

            PageCursorMoved += AvalonVisitingViewModel_PageCursorMoved;
            //TitleService = AppEngine.GetRequiredService<TitleService>();
            ExceptionService = AppEngine.GetRequiredService<ExceptionService>();
            ReadingSettings = AppEngine.GetRequiredService<AnfSettings>().Reading;
            readingSubscriber = ReadingSettings.Subscribe(x => x.LoadAll, OnReadingSettingsLoadAllChanged);
        }

        private void OnReadingSettingsLoadAllChanged()
        {
            if (ReadingSettings.LoadAll)
            {
                _ = LoadAllAsync();
            }
        }

        protected override void OnInitDone()
        {
            base.OnInitDone();
        }

        private void AvalonVisitingViewModel_PageCursorMoved(IDataCursor<IComicVisitPage<ImageSource>> arg1, int arg2)
        {
            SelectedResource = GetResource(arg2);
        }
        public async void SaveLogo()
        {
            var logo = LogoImage;
            if (logo != null)
            {
                var name = $"{PathHelper.EnsureName(Name)}-logo.jpg";
                //await logo.PickSaveAsync(name);
            }
        }
        public async void SaveImage(ComicPageInfo<ImageSource> info)
        {
            try
            {
                var name = $"{PathHelper.EnsureName(Name)}-{PathHelper.EnsureName(CurrentChapter.Title)}-{info.Index}.jpg";
                //await info.Resource.PickSaveAsync(name);
            }
            catch (Exception ex)
            {
                ExceptionService.Exception = ex;
            }
        }
        protected async override void OnCurrentChaterCursorChanged(IDataCursor<IComicChapterManager<ImageSource>> cursor)
        {
            try
            {
                base.OnCurrentChaterCursorChanged(cursor);
                //TitleService.Title = $"{ComicEntity.Name}";
                SelectedResource = null;
                OnPropertyChanged(nameof(TrulyCurrentComicChapter));
                if (ReadingSettings.LoadAll)
                {
                    await LoadAllAsync();
                }
                else
                {
                    await LoadResourceAsync(0);
                }
            }
            catch (Exception ex)
            {
                ExceptionService.Exception = ex;
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            //TitleService.Title = string.Empty;
            readingSubscriber.Dispose();
            PageCursorMoved -= AvalonVisitingViewModel_PageCursorMoved;
            GC.SuppressFinalize(this);
        }
    }
}
