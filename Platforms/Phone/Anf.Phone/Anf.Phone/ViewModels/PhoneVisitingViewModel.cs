using Anf.Easy.Visiting;
using Anf.Models;
using Anf.Phone.Models;
using Anf.Phone.Settings;
using Anf.Platform;
using Anf.Platform.Services;
using Anf.Platform.Settings;
using Anf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Anf.Phone.ViewModels
{
    public class PhoneVisitingViewModel : StoreBoxVisitingViewModel<ImageSource, ImageSource, PhoneComicStoreBox>
    {
        public static async Task<PhoneVisitingViewModel> CreateAsync(string address, bool usingStore = false)
        {
            var vm = new PhoneVisitingViewModel(x =>
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
        public PhoneVisitingViewModel(Func<IServiceProvider, IComicVisiting<ImageSource>> visiting = null) : base(visiting)
        {
            PhoneInit();
        }

        public PhoneVisitingViewModel(IComicVisiting<ImageSource> visiting, HttpClient httpClient, RecyclableMemoryStreamManager recyclableMemoryStreamManager, IStreamImageConverter<ImageSource> streamImageConverter, IObservableCollectionFactory observableCollectionFactory) : base(visiting, httpClient, recyclableMemoryStreamManager, streamImageConverter, observableCollectionFactory)
        {
            PhoneInit();
        }

        private IDisposable readingSubscriber;
        private ComicPageInfo<ImageSource> selectedResource;
        public ComicChapter TrulyCurrentComicChapter
        {
            get => this.CurrentChapter;
            set
            {
                _ = AvalonGoChapterAsync(value);
            }
        }
        public ComicPageInfo<ImageSource> SelectedResource
        {
            get { return selectedResource; }
            private set => Set(ref selectedResource, value);
        }


        public ReadingSettings ReadingSettings { get; private set; }

        internal ExceptionService ExceptionService { get; set; }

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
        private void PhoneInit()
        {
            PageCursorMoved += AvalonVisitingViewModel_PageCursorMoved;

            ReadingSettings = AppEngine.GetRequiredService<AnfSettings>().Reading;
            readingSubscriber = ReadingSettings.Subscribe(x => x.LoadAll, OnReadingSettingsLoadAllChanged);

        }

        private void AvalonVisitingViewModel_PageCursorMoved(IDataCursor<IComicVisitPage<ImageSource>> arg1, int arg2)
        {
            SelectedResource = GetResource(arg2);
        }
        private void OnReadingSettingsLoadAllChanged()
        {
            if (ReadingSettings.LoadAll)
            {
                _ = LoadAllAsync();
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
            readingSubscriber.Dispose();
            PageCursorMoved -= AvalonVisitingViewModel_PageCursorMoved;
        }
    }
}
