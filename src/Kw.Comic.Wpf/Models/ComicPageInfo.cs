using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kw.Comic.Visit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Kw.Comic.Wpf.Models
{
    public class ComicPageInfo : ObservableObject,IDisposable
    {

        public ComicPageInfo(ChapterVisitor visitor)
        {
            Visitor = visitor;
            LoadCommand = new RelayCommand(() => _ = LoadAsync());
        }
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private bool loading;
        private BitmapImage image;
        private bool error;
        private string errorMsg;
        private bool done;

        public bool Done
        {
            get { return done; }
            set => Set(ref done, value);
        }

        public string ErrorMsg
        {
            get { return errorMsg; }
            private set => Set(ref errorMsg, value);
        }

        public bool Error
        {
            get { return error; }
            private set => Set(ref error, value);
        }

        public BitmapImage Image
        {
            get { return image; }
            private set => Set(ref image, value);
        }

        public bool Loading
        {
            get { return loading; }
            private set => Set(ref loading, value);
        }

        public ChapterVisitor Visitor { get; }

        public ICommand LoadCommand { get; }

        public void Dispose()
        {
            semaphoreSlim.Dispose();
        }

        public async Task LoadAsync()
        {
            if (Image != null)
            {
                return;
            }
            await semaphoreSlim.WaitAsync();
            if (Image != null)
            {
                semaphoreSlim.Release();
                return;
            }
            Loading = true;
            try
            {
                await Visitor.LoadAsync();
                Visitor.Stream.Seek(0, SeekOrigin.Begin);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = Visitor.Stream;
                bitmap.EndInit();
                Image = bitmap;
                Done = true;
            }
            catch(Exception ex)
            {
                Error = true;
                ErrorMsg = ex.Message;
            }
            finally
            {
                Loading = false;
                semaphoreSlim.Release();
            }
        }
    }
}
