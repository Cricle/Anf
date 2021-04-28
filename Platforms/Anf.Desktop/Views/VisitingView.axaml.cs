using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Anf.Desktop.Services;
using Anf.Desktop.ViewModels;
using System;
using System.Diagnostics;
using Anf.Models;
using Avalonia.Media.Imaging;
using Avalonia.Input;
using Avalonia.VisualTree;
using System.Threading.Tasks;

namespace Anf.Desktop.Views
{
    public class VisitingView : UserControl
    {
        public VisitingView()
        {
            InitializeComponent();
            LoadVm("http://www.dm5.com/manhua-monvzhilv/");
        }
        public VisitingView(string address)
        {
            InitializeComponent();
            LoadVm(address);
        }
        private VisitingControlView vc;
        private TitleService titleService;
        private DesktopVisitingViewModel vm;
        private async void LoadVm(string address)
        {
            vm = new DesktopVisitingViewModel();
            DataContext = vm;
            vc = new VisitingControlView { DataContext = vm };
            titleService= AppEngine.GetRequiredService<TitleService>();
            titleService.LeftControls.Add(vc);
            try
            {
                await vm.Visiting.LoadAsync(address);
                if (vm.HasStoreBox)
                {
                    await vm.GoChapterIndexAsync(vm.StoreBox.AttackModel.CurrentChapter);
                }
                else
                {
                    await vm.NextChapterAsync();
                }
                _ = LoadPageAsync(0);
                vm.TransverseChanged += Vm_TransverseChanged;
                rep = this.Get<ItemsRepeater>("Rep");
                car = this.Get<Carousel>("Car");
                var sv = this.Get<ScrollViewer>("RepSv");
                sv.ScrollChanged += Sv_ScrollChanged;
                this.KeyDown += OnCarKeyDown;
                Vm_TransverseChanged(vm, vm.Transverse);
            }
            catch (Exception ex)
            {
                vm.ExceptionService.Exception = ex;
            }
        }
        private Task LoadPageAsync(int index)
        {
            var p = vm.GetResource(index);
            if (p is null)
            {
                return Task.CompletedTask;
            }
            return p.LoadAsync();
        }
        private async void Sv_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!vm.ReadingSettings.LoadAll)
            {
                var sv = (ScrollViewer)sender;
                if (sv.Extent.Height <= sv.Offset.Y + sv.Viewport.Height*2)
                {
                    var idx = vm.CurrentPageCursor.CurrentIndex;
                    await vm.LoadResourceAsync(idx + 1);
                    await vm.GoPageIndexAsync(idx + 1);
                }
            }
        }

        private async void OnCarKeyDown(object sender, KeyEventArgs e)
        {
            if (vm.Transverse)
            {
                if (e.Key== Key.Left)
                {
                    await vm.PrevPageAsync();
                }
                else if (e.Key== Key.Right)
                {
                    await vm.NextPageAsync();
                }
            }
        }

        private ItemsRepeater rep;
        private Carousel car;
        private void Vm_TransverseChanged(DesktopVisitingViewModel arg1, bool arg2)
        {
            if (arg2)
            {
                rep.ElementPrepared -= OnRepElementPrepared;
                rep.ElementIndexChanged -= OnRepElementIndexChanged;
                car.SelectionChanged += OnSelectionChanged;
                car.PointerReleased += OnCarPointerReleased;
            }
            else
            {
                rep.ElementPrepared += OnRepElementPrepared;
                rep.ElementIndexChanged += OnRepElementIndexChanged;
                car.SelectionChanged -= OnSelectionChanged;
                car.PointerReleased -= OnCarPointerReleased;
            }
        }

        private async void OnRepElementPrepared(object sender, ItemsRepeaterElementPreparedEventArgs e)
        {
            if (vm.ReadingSettings.LoadAll)
            {
                try
                {
                    await vm.GoPageIndexAsync(e.Index);
                }
                catch (Exception ex)
                {
                    vm.ExceptionService.Exception = ex;
                }
            }
        }

        private async void OnRepElementIndexChanged(object sender, ItemsRepeaterElementIndexChangedEventArgs e)
        {
            if (vm.ReadingSettings.LoadAll)
            {
                try
                {
                    await vm.GoPageIndexAsync(e.NewIndex);
                }
                catch (Exception ex)
                {
                    vm.ExceptionService.Exception = ex;
                }
            }
        }

        private async void OnCarPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var actualWidth = car.DesiredSize.Width;
            if (!double.IsNaN(actualWidth))
            {
                var pos = e.GetCurrentPoint(sender as IVisual);
                var left = actualWidth / 2;
                try
                {
                    if (pos.Position.X >= left)
                    {
                        await vm.NextPageAsync();
                    }
                    else
                    {
                        await vm.PrevPageAsync();
                    }
                }
                catch (Exception ex)
                {
                    vm.ExceptionService.Exception = ex;
                }
            }
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count != 0)
                {
                    var val = (ComicPageInfo<Bitmap>)e.AddedItems[0];
                    var index = vm.Resources.IndexOf(val);
                    await vm.GoPageIndexAsync(index);
                }
            }
            catch (Exception ex)
            {
                vm.ExceptionService.Exception = ex;
            }
        }

        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromLogicalTree(e);
            titleService.LeftControls.Remove(vc);
            vm.Dispose();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
