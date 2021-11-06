using Anf;
using Anf.Easy.Visiting;
using Anf.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Anf.Models
{
    public class ComicSourceInfo : ObservableObject
    {
        public ComicSourceInfo(ComicSnapshot snapshot,
            ComicSource source,
            IComicSourceCondition condition)
        {
            Snapshot = snapshot;
            Source = source;
            Condition = condition;
            CanParse = !(condition is null);
            WatchCommand = new RelayCommand(Watch);
            CopyCommand = new RelayCommand(Copy);
            OpenCommand = new AsyncRelayCommand(OpenAsync);

        }
        public bool CanParse { get; }

        public IComicSourceCondition Condition { get; }

        public ComicSnapshot Snapshot { get; }

        public ComicSource Source { get; }


        public ICommand WatchCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand OpenCommand { get; }

        public void Watch()
        {
            if (!(Condition is null))
            {
                var navSer = AppEngine.GetRequiredService<IComicTurnPageService>();
                navSer.GoSource(this);
            }
        }
        public void Copy()
        {
            var navSer = AppEngine.GetRequiredService<IPlatformService>();
            navSer.Copy(Source.TargetUrl);
        }
        public Task OpenAsync()
        {
            var navSer = AppEngine.GetRequiredService<IPlatformService>();
            return navSer.OpenAddressAsync(Source.TargetUrl);
        }
    }
}
