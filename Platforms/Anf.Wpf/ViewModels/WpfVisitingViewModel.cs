using GalaSoft.MvvmLight.Command;
using Kw.Comic.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Kw.Comic.Engine.Easy.Visiting;
using System.Windows.Media;
using Kw.Comic.Engine.Easy;
using Kw.Comic.ViewModels;
using Microsoft.IO;

namespace Kw.Comic.Wpf.ViewModels
{
    public partial class WpfVisitingViewModel : VisitingViewModel<ImageSource,ImageSource>, IDisposable
    {
        public static async Task<WpfVisitingViewModel> FromUriAsync(string uri)
        {
            var vm= new WpfVisitingViewModel();
            await vm.Visiting.LoadAsync(uri);
            return vm;
        }

        public WpfVisitingViewModel()
        {
            InitCommands();
        }

        public WpfVisitingViewModel(IComicVisiting<ImageSource> visiting, HttpClient httpClient, RecyclableMemoryStreamManager recyclableMemoryStreamManager, IStreamImageConverter<ImageSource> streamImageConverter) 
            : base(visiting, httpClient, recyclableMemoryStreamManager, streamImageConverter)
        {
            InitCommands();
        }

        private Visibility controlVisibility = Visibility.Visible;


        public Visibility ControlVisibility
        {
            get { return controlVisibility; }
            set => Set(ref controlVisibility, value);
        }
        public ICommand ExportCommand { get; private set; }
        public ICommand OpenComicCommand { get; private set; }
        public ICommand ToggleControlVisibilityCommand { get; private set; }

        public void ToggleControlVisibility()
        {
            ControlVisibility = ControlVisibility == Visibility.Visible ?
                 Visibility.Collapsed : Visibility.Visible;
        }

        private void InitCommands()
        {
            ExportCommand = new RelayCommand(ExportComicImage);
            OpenComicCommand = new RelayCommand(OpenComic);
            ToggleControlVisibilityCommand = new RelayCommand(ToggleControlVisibility);
        }
        public void OpenComic()
        {
            Process.Start(ComicEntity.ComicUrl);
        }
        public async void ExportComicImage()
        {
            var stream = LogoStream;
            if (stream is null)
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
                    stream.Seek(0, SeekOrigin.Begin);
                    await stream.CopyToAsync(fs);
                }
            }
        }
    }
}
