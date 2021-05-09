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
using Anf.Desktop.Services;
using Anf.Desktop.Models;
using Anf.Platform.Services;
using Microsoft.Extensions.DependencyInjection;
using Anf.Platform;
using Anf.Desktop.Settings;
using System.ComponentModel;
using Anf.Platform.Settings;

namespace Anf.Desktop.ViewModels
{
    public class DesktopVisitingViewModel : StoreBoxVisitingViewModel<Bitmap, Bitmap,AvalonComicStoreBox>
    {
        public static async Task<DesktopVisitingViewModel> CreateAsync(string address,bool usingStore=false)
        {
            var vm = new DesktopVisitingViewModel(x=> 
            {
                var visiting=x.GetRequiredService<StoreComicVisiting<Bitmap>>();
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
        public DesktopVisitingViewModel(Func<IServiceProvider,IComicVisiting<Bitmap>> visiting = null)
            : base(visiting)
        {
            AvalonInit();
        }

        public DesktopVisitingViewModel(IComicVisiting<Bitmap> visiting, HttpClient httpClient, RecyclableMemoryStreamManager recyclableMemoryStreamManager, IStreamImageConverter<Bitmap> streamImageConverter,IObservableCollectionFactory observableCollectionFactory)
            : base(visiting, httpClient, recyclableMemoryStreamManager, streamImageConverter, observableCollectionFactory)
        {
            AvalonInit();
        }
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
        private bool leftPaneOpen;
        private IDisposable readingSubscriber;

        public ReadingSettings ReadingSettings { get; private set; }
        public StretchMode[] StretchModes { get; private set; }
        public RelayCommand OpenPaneCommand { get; private set; }
        public RelayCommand ClosePaneCommand { get; private set; }
        public RelayCommand SaveLogoCommand { get; private set; }
        public RelayCommand<ComicPageInfo<Bitmap>> SaveImageCommand { get; private set; }

        internal TitleService TitleService { get; set; }
        internal ExceptionService ExceptionService { get; set; }

        public event Action<DesktopVisitingViewModel, bool> TransverseChanged;

        public bool LeftPaneOpen
        {
            get { return leftPaneOpen; }
            set => Set(ref leftPaneOpen, value);
        }

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
            StretchMode = StretchMode.UniformToFill;
            StretchModes = ZoomBorder.StretchModes;
            SaveImageCommand = new RelayCommand<ComicPageInfo<Bitmap>>(SaveImage);
            SaveLogoCommand = new RelayCommand(SaveLogo);
            OpenPaneCommand = new RelayCommand(OpenPane);
            ClosePaneCommand = new RelayCommand(ClosePane);

            PageCursorMoved += AvalonVisitingViewModel_PageCursorMoved;
            TitleService = AppEngine.GetRequiredService<TitleService>();
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
            TitleService.Title = $"Anf - {ComicEntity.Name}";
        }
        
        private void AvalonVisitingViewModel_PageCursorMoved(IDataCursor<IComicVisitPage<Bitmap>> arg1, int arg2)
        {
            SelectedResource = GetResource(arg2);
        }
        public async void SaveLogo()
        {
            var logo = LogoImage;
            if (logo!=null)
            {
                var name = $"{PathHelper.EnsureName(Name)}-logo.jpg";
                await logo.PickSaveAsync(name);
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
                base.OnCurrentChaterCursorChanged(cursor);
                TitleService.Title = $"{ComicEntity.Name}";
                SelectedResource = null;
                RaisePropertyChanged(nameof(TrulyCurrentComicChapter));
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
            TitleService.Title = string.Empty;
            readingSubscriber.Dispose();
            PageCursorMoved-= AvalonVisitingViewModel_PageCursorMoved;

        }
    }
}
