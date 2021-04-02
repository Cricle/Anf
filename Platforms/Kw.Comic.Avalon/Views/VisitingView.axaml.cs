using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kw.Comic.Avalon.ViewModels;
using System.Diagnostics;

namespace Kw.Comic.Avalon.Views
{
    public class VisitingView : UserControl
    {
        public VisitingView()
        {
            InitializeComponent();
            LoadVm();
        }
        private AvalonVisitingViewModel vm;
        private async void LoadVm()
        {
            
            vm = await AvalonVisitingViewModel.CreateAsync("https://manhua.dmzj.com/waixingmonv");
            await vm.NextChapterAsync();
            await vm.NextPageAsync();
            DataContext = vm;

            var rep = this.Get<ItemsRepeater>("Rep");
            rep.ElementClearing += Rep_ElementClearing;
            rep.ElementPrepared += Rep_ElementPrepared;
            rep.ElementIndexChanged += Rep_ElementIndexChanged;
        }

        private void Rep_ElementClearing(object sender, ItemsRepeaterElementClearingEventArgs e)
        {
            Debug.WriteLine(e.Element.GetType().FullName,"Clearing");
        }

        private async void Rep_ElementPrepared(object sender, ItemsRepeaterElementPreparedEventArgs e)
        {
            Debug.WriteLine(e.Index,"Prepared");
            var res = vm.Resources;
            if (e.Index < res.Count)
            {
                await vm.Resources[e.Index].LoadAsync();
            }
        }

        private void Rep_ElementIndexChanged(object sender, ItemsRepeaterElementIndexChangedEventArgs e)
        {
            Debug.WriteLine(e.NewIndex,"IndexChanged");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
