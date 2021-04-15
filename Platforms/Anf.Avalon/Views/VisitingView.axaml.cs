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

namespace Anf.Avalon.Views
{
    public class VisitingView : UserControl
    {
        public VisitingView()
        {
            InitializeComponent();
            LoadVm("https://ac.qq.com/Comic/comicInfo/id/530969");
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

            await vm.Visiting.LoadAsync(address);
            await vm.NextChapterAsync();
            var rep = this.Get<ItemsRepeater>("Rep");
            rep.ElementPrepared += OnElementPrepared;
        }


        private async void OnElementPrepared(object sender, ItemsRepeaterElementPreparedEventArgs e)
        {
            await vm.GoPageIndexAsync(e.Index);
            //Debug.WriteLine(e.Index,"Prepared");
            //var res = vm.Resources;
            //if (e.Index < res.Count)
            //{
            //    await vm.GoPageIndexAsync(e.Index);
            //    await res[e.Index].LoadAsync();
            //}
        }
        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromLogicalTree(e);
            titleService.LeftControls.Remove(vc);
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
