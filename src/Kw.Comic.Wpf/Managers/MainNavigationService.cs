using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kw.Comic.Wpf.Managers
{
    public class MainNavigationService
    {
        public const string AppName = "ANF";
        public Frame Frame { get; } = new Frame();

        public Window Window => App.Current.MainWindow;

        public void SetTitle(string info)
        {
            var win = Window;
            if (!string.IsNullOrEmpty(info))
            {
                win.Title = AppName + " - " + info;
            }
            else
            {
                win.Title = AppName;
            }
        }
    }
}
