using Anf.Cross.Settings;
using Anf.Easy.Visiting;
using Anf.Models;
using Anf.Platform;
using Anf.Platform.Services;
using Anf.Platform.Settings;
using Anf.ViewModels;
using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using Microsoft.Maui.Controls;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace Anf.Cross.ViewModels
{
    public class MAUIVisitingViewModel : VisitingViewModel<ImageResource, ImageSource>
    {
        public static async Task<MAUIVisitingViewModel> CreateAsync(string address, bool usingStore = false)
        {
            var vm = new MAUIVisitingViewModel(x =>
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
        public MAUIVisitingViewModel(IServiceProvider provider, Func<IServiceProvider, IComicVisiting<ImageSource>> visiting = null) : base(provider, visiting)
        {
            MAUIInit();
        }

        public MAUIVisitingViewModel(Func<IServiceProvider, IComicVisiting<ImageSource>> visiting = null, bool ignoreVisting = false) : base(visiting, ignoreVisting)
        {
            MAUIInit();
        }

        public MAUIVisitingViewModel(IComicVisiting<ImageSource> visiting, HttpClient httpClient, RecyclableMemoryStreamManager recyclableMemoryStreamManager, IStreamImageConverter<ImageSource> streamImageConverter, IObservableCollectionFactory observableCollectionFactory) : base(visiting, httpClient, recyclableMemoryStreamManager, streamImageConverter, observableCollectionFactory)
        {
            MAUIInit();
        }
        private ComicPageInfo<ImageSource> selectedResource;
        private IDisposable readingSubscriber;


        public ComicPageInfo<ImageSource> SelectedResource
        {
            get { return selectedResource; }
            private set => Set(ref selectedResource, value);
        }

        public ComicChapter TrulyCurrentComicChapter
        {
            get => this.CurrentChapter;
            set
            {
                _ = MAUIGoChapterAsync(value);
            }
        }
        public ReadingSettings ReadingSettings { get; private set; }
        public RelayCommand SaveLogoCommand { get; private set; }
        public RelayCommand<ComicPageInfo<ImageSource>> SaveImageCommand { get; private set; }

        internal ExceptionService ExceptionService { get; set; }

        private async Task MAUIGoChapterAsync(ComicChapter chapter)
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
        private void MAUIInit()
        {
            SaveImageCommand = new RelayCommand<ComicPageInfo<ImageSource>>(SaveImage);
            SaveLogoCommand = new RelayCommand(SaveLogo);

            PageCursorMoved += AvalonVisitingViewModel_PageCursorMoved;
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

        private void AvalonVisitingViewModel_PageCursorMoved(IDataCursor<IComicVisitPage<ImageSource>> arg1, int arg2)
        {
            SelectedResource = GetResource(arg2);
        }
        public async void SaveLogo()
        {
            var logo = LogoStream;
            if (logo!=null)
            {
                //var name = $"{PathHelper.EnsureName(Name)}-logo.jpg";
                await logo.PickSaveAsync();
            }
        }
        public async void SaveImage(ComicPageInfo<ImageSource> info)
        {
            try
            {
                var stream =await StoreFetchHelper.GetOrFromCacheAsync(info.Page.TargetUrl, () =>
                {
                    var eng = scope.ServiceProvider.GetRequiredService<ComicEngine>();
                    var selectEng = eng.GetComicSourceProviderType(info.PageSlots.ChapterManager.ChapterWithPage.Chapter.TargetUrl);
                    if (selectEng is null)
                    {
                        return Task.FromResult<Stream>(null);
                    }
                    var provider = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(selectEng.ProviderType);
                    return provider.GetImageStreamAsync(info.Page.TargetUrl);
                });
                if (stream!=null)
                {
                    using (stream)
                    {
                        //var name = $"{PathHelper.EnsureName(Name)}-{PathHelper.EnsureName(CurrentChapter.Title)}-{info.Index}.jpg";
                        await stream.PickSaveAsync();
                    }
                }
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
            scope?.Dispose();
            readingSubscriber.Dispose();
            PageCursorMoved -= AvalonVisitingViewModel_PageCursorMoved;
        }
    }
}