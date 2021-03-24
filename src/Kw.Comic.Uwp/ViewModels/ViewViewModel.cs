using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kw.Comic.Engine;
using Kw.Comic.Engine.Easy;
using Kw.Comic.Engine.Easy.Visiting;
using Kw.Comic.Render;
using Kw.Comic.Uwp.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;

namespace Kw.Comic.Uwp.ViewModels
{
    public class ViewViewModel : ViewViewModelBase<ImageSource, ImageSource>, IDisposable
    {
        public static async Task<ViewViewModel> FromUriAsync(string uri)
        {
            var scope = AppEngine.CreateScope();
            var eng = scope.ServiceProvider.GetRequiredService<IComicVisiting<ImageSource>>();
            var condtion = await eng.LoadAsync(uri);
            if (!condtion)
            {
                return null;
            }
            return new ViewViewModel(scope, eng);
        }

        public ViewViewModel(
            IServiceScope scope,IComicVisiting<ImageSource> visiting)
            :base(scope,visiting)
        {
            InitConverImage();

            GoBackCommand = new RelayCommand(GoBack);
            ExportCommand = new RelayCommand(ExportComicImage);
            OpenComicCommand = new RelayCommand(OpenComic);
        }
        private InMemoryRandomAccessStream convertStream;
        private Visibility controlVisibility= Visibility.Collapsed;

        public Visibility ControlVisibility
        {
            get { return controlVisibility; }
            set => Set(ref controlVisibility, value);
        }
        public ICommand GoBackCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand OpenComicCommand { get; }

        public async void OpenComic()
        {
            var uri = new Uri(ComicEntity.ComicUrl);

            await Launcher.LaunchUriAsync(uri);
        }

        public void GoBack()
        {
            if (Window.Current.Content is Frame frame&&frame.CanGoBack)
            {
                frame.GoBack();
            }
        }
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

            var name = PathHelper.EnsureName(ComicEntity.Name);
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
                using (var str = await client.GetAsync(new Uri(ComicEntity.ImageUrl)))
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
            catch (Exception)
            {

            }
        }

        public override void Dispose()
        {
            convertStream.Dispose();
            convertStream = null;
        }
    }
}
