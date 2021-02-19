using Kw.Comic.Engine;
using Kw.Comic.Visit;
using Kw.Comic.Wpf.Managers;
using Kw.Comic.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SourceChord.FluentWPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kw.Comic.Wpf.Views
{
    /// <summary>
    /// ReaderWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReaderWindow : AcrylicWindow
    {
        public ReaderWindow(WpfComicWatcher watcher)//ComicReadModel model
        {
            InitializeComponent();
            //var txt = File.ReadAllText("a.txt");
            //var pages = JsonConvert.DeserializeObject<ChapterWithPage[]>(txt);
            //var model = new ComicReadModel(
            //    pages,
            //    ComicConst.EngineDMZJ,
            //    ComicConst.EngineDMZJ,
            //    ComicConst.ImageEngineDMZJ)
            //{ ComicName = "唯一的正义" };
            var vm= new ReaderViewModel(watcher);
            DataContext = vm;
            Unloaded += (_, __) => vm.Dispose();
        }
    }
}
