using Anf.Easy;
using Anf.Easy.Visiting;
using Anf.Models;
using Anf.Platform;
using Anf.Platform.Models.Impl;
using Anf.Platform.Services;
using Anf.Platform.Settings;
using Anf.Services;
using Anf.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Anf.ViewModels
{
    internal class UnoVisitingViewModel : StoreBoxVisitingViewModel<ImageBox, ImageBox, WithImageComicStoreBox<ImageBox, ImageBox>>
    {
        public static async Task<UnoVisitingViewModel> CreateAsync(string address, bool usingStore = false)
        {
            var vm = new UnoVisitingViewModel(x =>
            {
                var visiting = x.GetRequiredService<StoreComicVisiting<ImageBox>>();
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
        public UnoVisitingViewModel(Func<IServiceProvider, IComicVisiting<ImageBox>> visiting = null)
            : base(visiting)
        {
            AvalonInit();
        }

        public UnoVisitingViewModel(IComicVisiting<ImageBox> visiting, HttpClient httpClient, RecyclableMemoryStreamManager recyclableMemoryStreamManager, IStreamImageConverter<ImageBox> streamImageConverter, IObservableCollectionFactory observableCollectionFactory)
            : base(visiting, httpClient, recyclableMemoryStreamManager, streamImageConverter, observableCollectionFactory)
        {
            AvalonInit();
        }

        private bool isVerticalRailEnabled = true;
        private bool isScrollInertiaEnabled = true;
        private bool isHorizontalScrollChainingEnabled;
        private bool isVerticalScrollChainingEnabled = true;
        private ZoomMode zoomMode= ZoomMode.Enabled;

        public ZoomMode ZoomMode
        {
            get => zoomMode;
            set => SetProperty(ref zoomMode, value);
        }

        public bool IsVerticalScrollChainingEnabled
        {
            get => isVerticalScrollChainingEnabled;
            set => SetProperty(ref isVerticalScrollChainingEnabled, value);
        }
        public bool IsHorizontalScrollChainingEnabled
        {
            get => isHorizontalScrollChainingEnabled;
            set => SetProperty(ref isHorizontalScrollChainingEnabled, value);
        }
        public bool IsVerticalRailEnabled
        {
            get => isVerticalRailEnabled;
            set => SetProperty(ref isVerticalRailEnabled, value);
        }
        public bool IsScrollInertiaEnabled
        {
            get => isScrollInertiaEnabled;
            set => SetProperty(ref isScrollInertiaEnabled, value);
        }

        private double minWidth;
        private double minHeight;
        private ComicPageInfo<ImageBox> selectedResource;
        private bool statusShow;
        private bool leftPaneOpen;
        private IDisposable readingSubscriber;

        public ReadingSettings ReadingSettings { get; private set; }
        public RelayCommand OpenPaneCommand { get; private set; }
        public RelayCommand ClosePaneCommand { get; private set; }
        public RelayCommand SaveLogoCommand { get; private set; }
        public RelayCommand<ComicPageInfo<ImageBox>> SaveImageCommand { get; private set; }

        //internal TitleService TitleService { get; set; }
        internal ExceptionService ExceptionService { get; set; }

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

        public ComicPageInfo<ImageBox> SelectedResource
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

        public async Task InitAsync(string address)
        {
            await Visiting.LoadAsync(address);
            if (HasStoreBox)
            {
                await GoChapterIndexAsync(StoreBox.AttackModel.CurrentChapter);
            }
            else
            {
                await NextChapterAsync();
            }
            if (ReadingSettings.LoadAll)
            {
                _ = LoadAllAsync();
            }
            else
            {
                _ = LoadPageAsync(0);
            }
        }
        public Task LoadPageAsync(int index)
        {
            var p = GetResource(index);
            if (p is null)
            {
                return Task.CompletedTask;
            }
            return p.LoadAsync();
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
            //StretchMode = StretchMode.UniformToFill;
            //StretchModes = ZoomBorder.StretchModes;
            SaveImageCommand = new RelayCommand<ComicPageInfo<ImageBox>>(SaveImage);
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

        private void AvalonVisitingViewModel_PageCursorMoved(IDataCursor<IComicVisitPage<ImageBox>> arg1, int arg2)
        {
            SelectedResource = GetResource(arg2);
        }
        public async void SaveLogo()
        {
            var logo = LogoImage;
            if (logo != null)
            {
                var name = $"{PathHelper.EnsureName(Name)}-logo.jpg";
                LogoStream.Position = 0;
                var fp = new FileSavePicker
                {
                    SuggestedFileName = name
                };
                var file = await fp.PickSaveFileAsync();
                if (file != null)
                {
                    await LogoStream.CopyToAsync(LogoStream);
                }
                //await logo.PickSaveAsync(name);
            }
        }
        public async void SaveImage(ComicPageInfo<ImageBox> info)
        {
            try
            {
                var name = $"{PathHelper.EnsureName(Name)}-{PathHelper.EnsureName(CurrentChapter.Title)}-{info.Index}.jpg";
                //await info.Resource.PickSaveAsync(name);
                var fp = new FileSavePicker
                {
                    SuggestedFileName = name
                };
                var file = await fp.PickSaveFileAsync();
                if (file != null)
                {
                    info.Resource.Stream.Position = 0;
                    await LogoStream.CopyToAsync(info.Resource.Stream);
                }
            }
            catch (Exception ex)
            {
                ExceptionService.Exception = ex;
            }
        }
        protected async override void OnCurrentChaterCursorChanged(IDataCursor<IComicChapterManager<ImageBox>> cursor)
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
