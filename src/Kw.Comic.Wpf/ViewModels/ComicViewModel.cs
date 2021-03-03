using GalaSoft.MvvmLight;
using Kw.Comic.Engine;
using Kw.Comic.Wpf.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf.ViewModels
{
    public class ComicViewModel : ViewModelBase,IDisposable
    {
        public ComicViewModel(ComicSnapshotInfo comic)
        {
            Comic = comic;
            locker = new SemaphoreSlim(1, 1);
            viewModelMap = new Dictionary<ComicSourceInfo, ViewViewModel>();
        }
        private readonly SemaphoreSlim locker;
        private readonly Dictionary<ComicSourceInfo, ViewViewModel> viewModelMap;
        private ComicSourceInfo currentSource;
        private ViewViewModel viewViewModel;

        public ViewViewModel ViewViewModel
        {
            get { return viewViewModel; }
            set => Set(ref viewViewModel, value);
        }

        public ComicSourceInfo CurrentSource
        {
            get { return currentSource; }
            set
            {
                Set(ref currentSource, value);
                if (value!=null)
                {
                    Load(value);
                }
            }
        }       

        public ComicSnapshotInfo Comic { get; }

        private async void Load(ComicSourceInfo source)
        {

            if (viewModelMap.TryGetValue(source,out var vm))
            {
                ViewViewModel = vm;
                return;
            }
            await locker.WaitAsync();
            try
            {
                if (viewModelMap.TryGetValue(source, out vm))
                {
                    ViewViewModel = vm;
                    return;
                }
                vm = await ViewViewModel.FromUriAsync(source.Source.TargetUrl);
                viewModelMap.Add(source, vm);
                ViewViewModel = vm;
            }
            finally
            {
                locker.Release();
            }
        }

        public void Dispose()
        {
            foreach (var item in viewModelMap)
            {
                if (item.Value!= ViewViewModel)
                {
                    item.Value.Dispose();
                }
            }
            locker.Dispose();
        }
    }
}
