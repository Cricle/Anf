using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kw.Comic.Engine;
using Kw.Comic.Uwp.Managers;
using Kw.Comic.Visit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Streams;
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
        }
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IServiceScope scope;

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

        private async void InitConverImage()
        {
            try
            {
                using (var client = new Windows.Web.Http.HttpClient())
                using (var str = await client.GetAsync(new Uri(Watcher.Comic.ImageUrl)))
                using (var mem = new InMemoryRandomAccessStream())
                {
                    var buffer=await str.Content.ReadAsBufferAsync();
                    await mem.WriteAsync(buffer);
                    var bm = new BitmapImage();
                    mem.Seek(0);
                    await bm.SetSourceAsync(mem);
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
        }
    }
}
