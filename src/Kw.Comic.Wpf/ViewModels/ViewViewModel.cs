using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kw.Comic.Engine;
using Kw.Comic.Visit;
using Kw.Comic.Wpf.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Kw.Comic.Wpf.ViewModels
{
    public class ViewViewModel : ViewModelBase,IDisposable
    {
        public static async Task<ViewViewModel> FromUriAsync(string uri)
        {
            var scope = WpfAppEngine.Instance.CreateScope();
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

            Watcher = new WpfComicWatcher(scope,entity,
                httpClientFactory,
                condition, provider);
            ComicVisitors = Watcher.ChapterCursor.Datas;
            InitConverImage();

            NextChapterCommand = new RelayCommand(NextChapter);
            PrevChapterCommand = new RelayCommand(PrevChapter);
            ExportCommand = new RelayCommand(ExportComicImage);
            OpenComicCommand = new RelayCommand(OpenComic);
        }
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IServiceScope scope;
        private MemoryStream convertStream;

        private Visibility controlVisibility = Visibility.Collapsed;

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
                _ = Watcher.ToChapterAsync(ChapterIndex);
            }
        }

        public BitmapImage ConverImage
        {
            get { return converImage; }
            private set => Set(ref converImage, value);
        }

        public int ChapterIndex => ComicVisitors.IndexOf(CurrentComicVisitor);

        public WpfComicWatcher Watcher { get; }

        public ComicEntity ComicEntity { get; }

        public ImmutableArray<ComicVisitor> ComicVisitors { get; }

        public ICommand NextChapterCommand { get; }
        public ICommand PrevChapterCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand OpenComicCommand { get; }

        public void OpenComic()
        {
            Process.Start(ComicEntity.ComicUrl);
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
            if (index > 0)
            {
                CurrentComicVisitor = ComicVisitors[index - 1];
            }
        }
        private readonly HashSet<char> unsuoop = new HashSet<char>(Path.GetInvalidFileNameChars());
        public async void ExportComicImage()
        {
            if (convertStream == null)
            {
                return;
            }
            var picker = new SaveFileDialog();
            var name = new string(ComicEntity.Name.Select(x => unsuoop.Contains(x) ? '_' : x).ToArray());
            picker.FileName = name + ".jpg";
            picker.Filter = "图像文件(*.jpg)|*.jpg";
            if (picker.ShowDialog().GetValueOrDefault(false))
            {
                using (var fs = picker.OpenFile())
                {
                    convertStream.Seek(0, SeekOrigin.Begin);
                    await convertStream.CopyToAsync(fs);
                }
            }
        }

        private async void InitConverImage()
        {
            try
            {
                using (var client = new HttpClient())
                using (var str = await client.GetAsync(new Uri(Watcher.Comic.ImageUrl)))
                {
                    convertStream = new MemoryStream();
                    var buffer = await str.Content.ReadAsStreamAsync();
                    await buffer.CopyToAsync(convertStream);
                    convertStream.Seek(0, SeekOrigin.Begin);
                    var bm = new BitmapImage();
                    bm.BeginInit();
                    bm.CacheOption = BitmapCacheOption.OnLoad;
                    bm.StreamSource=convertStream;
                    bm.EndInit();
                    ConverImage = bm;
                }
            }
            catch (Exception ex)
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
