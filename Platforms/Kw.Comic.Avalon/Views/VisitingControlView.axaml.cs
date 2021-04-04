using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Kw.Comic.Avalon.Services;
using Kw.Comic.Avalon.ViewModels;
using Kw.Comic.Services;
using System;

namespace Kw.Comic.Avalon.Views
{
    public class VisitingControlView : StackPanel
    {
        private readonly TitleService titleService;
        public VisitingControlView()
        {
            InitializeComponent();
            titleService = AppEngine.GetRequiredService<TitleService>();
            titleService.GoBackButton.IsVisible = true;
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            foreach (var item in Children)
            {
                if (item is Button btn)
                {
                    btn.Bind(TemplatedControl.FontSizeProperty, new Binding(nameof(TitleService.AdviseFontSize)) { Source = titleService });
                }
            }
            this.Get<Button>("ChapterSelector").Click += GoChapterClick;
            //this.Get<ListBox>("ChapterBoxs").SelectionChanged += VisitingControlView_SelectionChanged;
        }

        //private async void VisitingControlView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (DataContext is AvalonVisitingViewModel vm&&e.AddedItems.Count!=0&&e.AddedItems[0] is ComicChapter chapter&&vm.CurrentChapter!=chapter)
        //    {
        //        await vm.GoChapterAsync(chapter);
        //    }
        //}

        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromLogicalTree(e);
            titleService.GoBackButton.IsVisible = false;
        }
        private void GoChapterClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is AvalonVisitingViewModel vm)
            {
                vm.ChapterSelectorOpen = true;
            }
        }
    }
}
