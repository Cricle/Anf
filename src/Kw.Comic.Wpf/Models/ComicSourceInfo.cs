using GalaSoft.MvvmLight.Command;
using Kw.Comic.Engine;
using Kw.Comic.Wpf.Managers;
using Kw.Comic.Wpf.Views.Pages;
using MahApps.Metro.IconPacks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Kw.Comic.Wpf.Models
{
    public class ComicSourceInfo
    {
        public ComicSourceInfo()
        {
            WatchCommand = new RelayCommand(Watch);
            CopyCommand = new RelayCommand(Copy);
            OpenCommand = new RelayCommand(Open);
        }
        public bool CanParse => Condition != null;

        public PackIconMaterialLightKind Icon => CanParse ? PackIconMaterialLightKind.Check : PackIconMaterialLightKind.AlertCircle;

        public IComicSourceCondition Condition { get; set; }

        public string ComicName { get; set; }

        public ComicSource Source { get; set; }


        public ICommand WatchCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand OpenCommand { get; }

        public void Watch()
        {
            if (CanParse)
            {
                var navSer = AppEngine.GetRequiredService<MainNavigationService>();
                var viewPage = new ViewPage(Source.TargetUrl);
                navSer.Frame.Navigate(viewPage);
                navSer.SetTitle($"正在阅读{ComicName} - {Source.Name}");
            }
        }
        public void Copy()
        {
            if (CanParse)
            {
                Clipboard.SetText(Source.TargetUrl);
            }
        }
        public void Open()
        {
            if (CanParse)
            {
                var uri = Source.TargetUrl;
                Process.Start(uri);
            }
        }
    }
}
