using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kw.Comic.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Kw.Comic.Engine.Easy.Visiting;
using Microsoft.IO;
using System.Windows.Media;
using Kw.Comic.Engine.Easy;
using Kw.Comic.Render;

namespace Kw.Comic.Wpf.ViewModels
{
    public partial class ViewViewModel : ViewViewModelBase<ImageSource,ImageSource>, IDisposable
    {
        public static async Task<ViewViewModel> FromUriAsync(string uri)
        {
            var scope = AppEngine.CreateScope();
            var eng = scope.ServiceProvider.GetRequiredService<IComicVisiting<ImageSource>>();
            var ok = await eng.LoadAsync(uri);
            if (!ok)
            {
                return null;
            }
            return new ViewViewModel(scope, eng);
        }

        public ViewViewModel(IServiceScope scope, IComicVisiting<ImageSource> visiting)
            :base(scope,visiting)
        {
            InitConverImage();

            ExportCommand = new RelayCommand(ExportComicImage);
            OpenComicCommand = new RelayCommand(OpenComic);
            ToggleControlVisibilityCommand = new RelayCommand(ToggleControlVisibility);
        }
        private MemoryStream convertStream;
        private Visibility controlVisibility = Visibility.Visible;


        public Visibility ControlVisibility
        {
            get { return controlVisibility; }
            set => Set(ref controlVisibility, value);
        }
        public ICommand ExportCommand { get; }
        public ICommand OpenComicCommand { get; }
        public ICommand ToggleControlVisibilityCommand { get; }

        public void ToggleControlVisibility()
        {
            ControlVisibility = ControlVisibility == Visibility.Visible ?
                 Visibility.Collapsed : Visibility.Visible;
        }

        public void OpenComic()
        {
            Process.Start(ComicEntity.ComicUrl);
        }
        public async void ExportComicImage()
        {
            if (convertStream == null)
            {
                return;
            }
            var picker = new SaveFileDialog();
            var name = PathHelper.EnsureName(ComicEntity.Name);
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
                var client = AppEngine.Provider.GetRequiredService<HttpClient>();
                using (var str = await client.GetAsync(new Uri(ComicEntity.ImageUrl)))
                {
                    convertStream = recyclableMemoryStreamManager.GetStream();
                    var buffer = await str.Content.ReadAsStreamAsync();
                    await buffer.CopyToAsync(convertStream);
                    ConverImage = convertStream.AsBitmapImage(true);
                }
            }
            catch (Exception)
            {

            }
        }
        public override void Dispose()
        {
            base.Dispose();
            convertStream?.Dispose();
        }
    }
}
