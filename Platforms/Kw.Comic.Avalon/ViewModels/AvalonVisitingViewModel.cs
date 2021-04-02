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
        protected override async void OnCurrentChaterCursorChanged(IDataCursor<IComicChapterManager<Bitmap>> cursor)
        {
            //try
            //{
            //    await LoadAllAsync();
            //}
            //catch (Exception) { }
        }
    }
}
