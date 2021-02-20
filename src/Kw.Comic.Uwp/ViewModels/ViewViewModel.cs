using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kw.Comic.Engine;
using Kw.Comic.Uwp.Managers;
using Kw.Comic.Visit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;

namespace Kw.Comic.Uwp.ViewModels
{
    public class ViewViewModel : ViewModelBase, IDisposable
    {
        public static async Task<ViewViewModel> FromUriAsync(string uri)
        {
            var scope = UwpAppEngine.Instance.CreateScope();
            var eng = scope.ServiceProvider.GetRequiredService<ComicEngine>();
            var condtion = eng.GetComicSourceProviderType(uri);
            if (condtion == null)
            {
                return null;
            }
            var provider = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(condtion.ProviderType);
            var entity = await provider.GetChaptersAsync(uri);
            if (entity == null)
            {
                return null;
            }
            return new ViewViewModel(scope, entity, condtion, provider);
        }

        public ViewViewModel(
            IServiceScope scope,
            ComicEntity entity,
            IComicSourceCondition condition,
            IComicSourceProvider provider)
        {
            this.scope = scope;
            httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            ComicEntity = entity;

            Watcher = new UwpComicWatcher(scope,
                entity,
                httpClientFactory,
                condition, provider);
            ComicVisitors = Watcher.ChapterCursor.Datas;
            InitConverImage();

            NextChapterCommand = new RelayCommand(NextChapter);
            PrevChapterCommand = new RelayCommand(PrevChapter);
            GoBackCommand = new RelayCommand(GoBack);
            ExportCommand = new RelayCommand(ExportComicImage);
            OpenComicCommand = new RelayCommand(OpenComic);
        }
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IServiceScope scope;
        private InMemoryRandomAccessStream convertStream;

        private Visibility controlVisibility= Visibility.Collapsed;

        public Visibility ControlVisibility
        {
            get { return controlVisibility; }
            set => Set(ref controlVisibility, value);
        }
        private BitmapImage converImage;
        private ComicVisitor currentComicVisitor;

        public ComicVisitor CurrentComicVisitor
        {
            get { return currentComicVisitor; }
            set
            {
                Set(ref currentComicVisitor, value);
                _=Watcher.ToChapterAsync(ChapterIndex);
            }
        }

        public BitmapImage ConverImage
        {
            get { return converImage; }
            private set => Set(ref converImage, value);
        }

        public int ChapterIndex => ComicVisitors.IndexOf(CurrentComicVisitor);

        public UwpComicWatcher Watcher { get; }

        public ComicEntity ComicEntity { get; }

        public ImmutableArray<ComicVisitor> ComicVisitors { get; }

        public ICommand NextChapterCommand { get; }
        public ICommand PrevChapterCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand OpenComicCommand { get; }

        public async void OpenComic()
        {
            var uri = new Uri(ComicEntity.ComicUrl);

            await Launcher.LaunchUriAsync(uri);
        }

        public void NextChapter()
        {
            var index = ChapterIndex;
            if (index < ComicVisitors.Length)
            {
                CurrentComicVisitor = ComicVisitors[index + 1];
            }
        }
        public void PrevChapter()
        {
            var index = ChapterIndex;
            if (index>0)
            {
                CurrentComicVisitor = ComicVisitors[index - 1];
            }
        }
        public void GoBack()
        {
            if (Window.Current.Content is Frame frame&&frame.CanGoBack)
            {
                frame.GoBack();
            }
        }
        private readonly HashSet<char> unsuoop = new HashSet<char>(Path.GetInvalidFileNameChars());
        public async void ExportComicImage()
        {
            if (convertStream==null)
            {
                return;
            }
            var picker = new FileSavePicker();
            picker.DefaultFileExtension = ".png";
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeChoices.Add("Images", new string[] { ".png" });
            var name = new string(Watcher.Comic.Name.Select(x => unsuoop.Contains(x) ? '_' : x).ToArray());
            picker.SuggestedFileName = $"{name}.png";
            var file =await picker.PickSaveFileAsync();
            if (file!=null)
            {
                using (var s = await file.OpenAsync(FileAccessMode.ReadWrite))
                using (var fs = s.AsStreamForWrite())
                {
                    convertStream.Seek(0);
                    var convers = convertStream.AsStreamForRead();
                    await convers.CopyToAsync(fs);
                }
            }
        }

        private async void InitConverImage()
        {
            try
            {
                using (var client = new Windows.Web.Http.HttpClient())
                using (var str = await client.GetAsync(new Uri(Watcher.Comic.ImageUrl)))
                {
                    convertStream = new InMemoryRandomAccessStream();
                    var buffer=await str.Content.ReadAsBufferAsync();
                    await convertStream.WriteAsync(buffer);
                    var bm = new BitmapImage();
                    convertStream.Seek(0);
                    await bm.SetSourceAsync(convertStream);
                    ConverImage = bm;
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void Dispose()
        {
            scope.Dispose();
            convertStream.Dispose();
            convertStream = null;
        }
    }
}
