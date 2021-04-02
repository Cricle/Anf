using Avalonia.Media.Imaging;
using Kw.Comic.Engine.Easy.Visiting;
using Kw.Comic.ViewModels;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon.ViewModels
{
    public class AvalonVisitingViewModel : VisitingViewModel<Bitmap, Bitmap>
    {
        public static async Task<AvalonVisitingViewModel> CreateAsync(string address)
        {
            var vm = new AvalonVisitingViewModel();
            var ok=await vm.Visiting.LoadAsync(address);
            if (ok)
            {
                vm.Init();
            }
            return vm;
        }
        public AvalonVisitingViewModel()
            :base(null)
        {
        }
        public AvalonVisitingViewModel(IComicVisiting<Bitmap> visiting = null) 
            : base(visiting)
        {
        }

        public AvalonVisitingViewModel(IComicVisiting<Bitmap> visiting, HttpClient httpClient, RecyclableMemoryStreamManager recyclableMemoryStreamManager, IStreamImageConverter<Bitmap> streamImageConverter)
            : base(visiting, httpClient, recyclableMemoryStreamManager, streamImageConverter)
        {
        }
        public SilentObservableCollection<IComicVisitPage<Bitmap>> Bitmaps { get; } = new SilentObservableCollection<IComicVisitPage<Bitmap>>();

        protected override void OnCurrentPageCursorChanged(IDataCursor<IComicVisitPage<Bitmap>> cursor)
        {
            Bitmaps.Clear();
            LoadPage(cursor, PageSlots.ToLoadEnumerable().ToArray());
        }

        private async void LoadPage(IDataCursor<IComicVisitPage<Bitmap>> cursor,Func<Task<IComicVisitPage<Bitmap>>>[] tasks)
        {
            for (int i = 0; i < tasks.Length && cursor == CurrentPageCursor; i++)
            {
                var ds = await tasks[i]();
                if (ds!=null&&cursor == CurrentPageCursor)
                {
                    Bitmaps.Add(ds);
                }
            }
        }
    }
}
