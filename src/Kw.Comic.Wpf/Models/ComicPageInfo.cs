using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kw.Comic.Visit;
using Kw.Comic.Wpf.Managers;
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
            semaphoreSlim = new SemaphoreSlim(1,1);
            LoadCommand = new RelayCommand(() => _ = LoadAsync());
        }

        private readonly SemaphoreSlim semaphoreSlim;
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
            Visitor.Dispose();
        }
        public async Task UnLoadAsync()
        {
            if (Image==null)
            {
                return;
            }
            try
            {
                await semaphoreSlim.WaitAsync();
                if (Image == null)
                {
                    return;
                }
                Image = null;
                Done = false;
            }
            catch(Exception ex)
            {
                Error = true;
                ErrorMsg = ex.Message;
            }
            finally
            {
                try
                {
                    semaphoreSlim.Release();
                }
                catch (Exception) { }
            }
        }
        public async Task LoadAsync()
        {
            if (Image != null)
            {
                return;
            }
            try
            {
                await semaphoreSlim.WaitAsync();
                if (Image != null)
                {
                    return;
                }
                Loading = true;
                await Visitor.LoadAsync();
                Visitor.Stream.Seek(0, SeekOrigin.Begin);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = Visitor.Stream;
                bitmap.EndInit();
                bitmap.Freeze();
                await Visitor.UnLoadAsync();
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
                try
                {
                    semaphoreSlim.Release();
                }
                catch (Exception) { }
            }
        }
    }
}
