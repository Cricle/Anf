using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Anf.Avalon.Services;
using Anf.Avalon.ViewModels;
using System;
using System.Diagnostics;
using Anf.Models;
using Avalonia.Media.Imaging;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace Anf.Avalon.Views
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
        private AvalonVisitingViewModel vm;
        private async void LoadVm(string address)
        {
            vm = new AvalonVisitingViewModel();
            DataContext = vm;
            vc = new VisitingControlView { DataContext = vm };
            titleService= AppEngine.GetRequiredService<TitleService>();
            titleService.LeftControls.Add(vc);
            try
            {
                await vm.Visiting.LoadAsync(address);
                await vm.NextChapterAsync();
                vm.TransverseChanged += Vm_TransverseChanged;
                rep = this.Get<ItemsRepeater>("Rep");
                car = this.Get<Carousel>("Car");
                this.KeyDown += Car_KeyDown;
                Vm_TransverseChanged(vm, vm.Transverse);
            }
            catch (Exception ex)
            {
                vm.ExceptionService.Exception = ex;
            }
        }
        private async void Car_KeyDown(object sender, KeyEventArgs e)
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
        private void Vm_TransverseChanged(AvalonVisitingViewModel arg1, bool arg2)
        {
            if (arg2)
            {
                rep.ElementPrepared -= OnElementPrepared;
                car.SelectionChanged += OnSelectionChanged;
                car.PointerReleased += OnCarPointerReleased;
            }
            else
            {
                rep.ElementPrepared += OnElementPrepared;
                car.SelectionChanged -= OnSelectionChanged;
                car.PointerReleased -= OnCarPointerReleased;
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

        private async void OnElementPrepared(object sender, ItemsRepeaterElementPreparedEventArgs e)
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
