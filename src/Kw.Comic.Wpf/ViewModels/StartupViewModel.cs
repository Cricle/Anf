using GalaSoft.MvvmLight;
#if !NET461
using GalaSoft.MvvmLight.Command;
#else
using GalaSoft.MvvmLight.CommandWpf;
#endif
using Kw.Comic.Engine;
using Kw.Comic.Visit;
using Kw.Comic.Wpf.Managers;
using Kw.Comic.Wpf.Views;
using Kw.Core.Annotations;
using Kw.Core.Commands;
using Kw.Core.Commands.Builders;
using Kw.Core.Commands.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Kw.Comic.Wpf.ViewModels
{
    public class StartupViewModel : ObservableObject,IDisposable
    {
        public static readonly string BeginText = "可直接输入网站或文字以搜索漫画";

        private readonly ComicEngine engine;

        public StartupViewModel()
        {
            scope = WpfAppEngine.Instance.GetScope();
            CommandManager = scope.ServiceProvider.GetRequiredService<ICommandManager>();
            engine = scope.ServiceProvider.GetRequiredService<ComicEngine>();
            searchEngine = scope.ServiceProvider.GetRequiredService<SearchEngine>();
            ChapterWithPages = new ObservableCollection<ChapterWithPage>();
            Snapshots = new ObservableCollection<ComicSnapshot>();

            LoadCommand = new RelayCommand(Load);
            CancelCommand = new RelayCommand(Cancel);
            SeachCommand = new RelayCommand(Search);
            DownSeachCommand = new RelayCommand(DownSeachResult);
            UpSeachCommand = new RelayCommand(UpSeachResult);
            LeftSourceCommand = new RelayCommand(LeftSource);
            RightSourceCommand = new RelayCommand(RightSource);
        }
        private readonly IServiceScope scope;
        private readonly SearchEngine searchEngine;
        private CancellationTokenSource source;

        private string address;
        private bool extensionToTitle=true;
        private string tip= BeginText;
        private bool enable=true;
        private int totalChapter;
        private Visibility executeVisibility= Visibility.Collapsed;
        private Visibility inputVisibility;
        private int totalComic;
        private Visibility searchVisibility= Visibility.Collapsed;
        private ComicSnapshot currentSnapshot;
        private ComicSource currentSource;

        public ComicSource CurrentSource
        {
            get { return currentSource; }
            set 
            {
                Set(ref currentSource, value);
                try
                {
                    if (value != null)
                    {
                        var type = engine.GetComicSourceProviderType(value.TargetUrl);
                        if (type != null)
                        {
                            Address = value.TargetUrl;
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        public ComicSnapshot CurrentSnapshot
        {
            get { return currentSnapshot; }
            set
            {
                Set(ref currentSnapshot, value);
                CurrentSource = value?.Sources?.FirstOrDefault();
                
            }
        }

        public Visibility SearchVisibility
        {
            get { return searchVisibility; }
            set => Set(ref searchVisibility, value);
        }

        public int TotalComic
        {
            get { return totalComic; }
            set => Set(ref totalComic, value);
        }

        public Visibility InputVisibility
        {
            get { return inputVisibility; }
            set => Set(ref inputVisibility, value);
        }


        public Visibility ExecuteVisibility
        {
            get { return executeVisibility; }
            set => Set(ref executeVisibility, value);
        }

        public int TotalChapter
        {
            get { return totalChapter; }
            set => Set(ref totalChapter, value);
        }

        public bool Enable
        {
            get { return enable; }
            set => Set(ref enable, value);
        }

        public string Tip
        {
            get { return tip; }
            set => Set(ref tip, value);
        }

        public bool ExtensionToTitle
        {
            get { return extensionToTitle; }
            set => Set(ref extensionToTitle, value);
        }

        public string Address
        {
            get { return address; }
            set
            {
                Set(ref address, value);
                if (string.IsNullOrEmpty(value))
                {
                    Tip = BeginText;
                    SearchVisibility = Visibility.Collapsed;
                }
                else
                {
                    try
                    {
                        var type = engine.GetComicSourceProviderType(Address);
                        if (type==null)
                        {
                            Tip = $"正在搜索{value}";
                            Search();
                            SearchVisibility = Visibility.Visible;
                        }
                        else
                        {
                            Tip = $"将会使用[{type.EnginName}]引擎";
                            SearchVisibility = Visibility.Collapsed;
                        }
                    }
                    catch (Exception)
                    {
                        Tip = $"正在搜索{value}";
                        Search();
                        SearchVisibility = Visibility.Visible;
                    }
                }
            }
        }

        public ICommandManager CommandManager { get; }

        public ICommand UpdateCommandVisibilityCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SeachCommand { get; }
        public ICommand DownSeachCommand { get; }
        public ICommand UpSeachCommand { get; }
        public ICommand LeftSourceCommand { get; }
        public ICommand RightSourceCommand { get; }

        public ObservableCollection<ChapterWithPage> ChapterWithPages { get; }

        public ObservableCollection<ComicSnapshot> Snapshots { get; }

        public void RightSource()
        {
            if (CurrentSnapshot == null)
            {
                CurrentSource = null;
                return;
            }
            if (CurrentSource == null || CurrentSnapshot.Sources.Length == 0)
            {
                CurrentSource = CurrentSnapshot.Sources.FirstOrDefault();
                return;
            }
            var index = Array.IndexOf(CurrentSnapshot.Sources, CurrentSource);
            if (index >= CurrentSnapshot.Sources.Length-1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
            CurrentSource = CurrentSnapshot.Sources[index];
        }

        public void LeftSource()
        {
            if (CurrentSnapshot==null)
            {
                CurrentSource = null;
                return;
            }
            if (CurrentSource==null||CurrentSnapshot.Sources.Length==0)
            {
                CurrentSource = CurrentSnapshot.Sources.FirstOrDefault();
                return;
            }
            var index = Array.IndexOf(CurrentSnapshot.Sources, CurrentSource);
            if (index<=0)
            {
                index = CurrentSnapshot.Sources.Length-1;
            }
            else
            {
                index--;
            }
            CurrentSource = CurrentSnapshot.Sources[index];
        }

        public void UpSeachResult()
        {
            if (CurrentSnapshot == null)
            {
                CurrentSnapshot = Snapshots.LastOrDefault();
            }
            if (Snapshots.Count == 0)
            {
                return;
            }
            var index = Snapshots.IndexOf(CurrentSnapshot);
            if (index >=Snapshots.Count-1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
            CurrentSnapshot = Snapshots[index];
        }
        public void DownSeachResult()
        {
            if (CurrentSnapshot==null)
            {
                CurrentSnapshot = Snapshots.FirstOrDefault();
            }
            if (Snapshots.Count==0)
            {
                return;
            }
            var index = Snapshots.IndexOf(CurrentSnapshot);
            if (index<=0)
            {
                index = Snapshots.Count - 1;
            }
            else
            {
                index--;
            }
            CurrentSnapshot = Snapshots[index];
        }

        public async void Search()
        {
            CurrentSnapshot = null;
            Snapshots.Clear();
            var res= await searchEngine.SearchAsync(Address, 1, 30);
            TotalComic = res.Total ?? 0;
            Snapshots.AddRange(res.Snapshots);
            Tip = $"共获得{TotalComic}个结果";
        }

        public void Cancel()
        {
            source?.Cancel();
        }

        public async void Load()
        {
            source = new CancellationTokenSource();
            Enable = false;
            try
            {
                ChapterWithPages.Clear();
                TotalChapter = 0;
                InputVisibility = Visibility.Collapsed;
                ExecuteVisibility = Visibility.Visible;
                Tip = "正在寻找引擎";
                var type = engine.GetComicSourceProviderType(Address);
                if (type != null)
                {
                    source.Token.ThrowIfCancellationRequested();
                    var scope = WpfAppEngine.Instance.GetScope();
                    //using ()
                    {
                        Tip = "正在获取章节";
                        var val = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(type.ProviderType);
                        var caps = await val.GetChaptersAsync(Address);
                        TotalChapter = caps.Chapters.Length;
                        Tip = "完成";
                        //await Task.Delay(1500);
                        var win = App.Current.MainWindow;
                        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                        var v = new WpfComicWatcher(scope, caps, httpClientFactory, type, val);
                        new ReaderWindow(v).Show();
                        win.Close();

                    }
                }
                else
                {
                    Tip = "该地址无支持引擎";
                }
            }
            catch (OperationCanceledException)
            {
                Tip = "已取消";
                await Task.Delay(1500);
            }
            finally
            {
                Enable = true;
                InputVisibility = Visibility.Visible;
                ExecuteVisibility = Visibility.Collapsed;
                source?.Dispose();
                source = null;
                Tip = BeginText;
            }
        }

        public void Dispose()
        {
            scope.Dispose();
        }
    }
}
