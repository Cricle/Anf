using GalaSoft.MvvmLight;
#if !NET461
using GalaSoft.MvvmLight.Command;
#else
using GalaSoft.MvvmLight.CommandWpf;
#endif
using Kw.Comic.Visit;
using Kw.Comic.Wpf.Managers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Kw.Comic.Wpf.ViewModels
{
    public class ReaderViewModel : ObservableObject, IDisposable
    {
        public ReaderViewModel(WpfComicWatcher wpfComicVisitor)
        {
            WpfComicVisitor = wpfComicVisitor;
            scope = WpfAppEngine.Instance.GetScope();

            ZoomAddCommand = new RelayCommand(ZoomAdd);
            ZoomSubCommand = new RelayCommand(ZoomSub);
            ShowInstructionsCommand = new RelayCommand(ShowInstructions);
            HideInstructionsCommand = new RelayCommand(HideInstructions);
            SetScalingModeCommand = new RelayCommand<BitmapScalingMode>(SetScalingMode);
            DownloadAllCommand = new RelayCommand(async () =>
            {
                DownloadVisibility = Visibility.Visible;
                try
                {
                    await wpfComicVisitor.DownloadAllAsync(new DownloadAllOptions
                    {
                        CachePageCursor = true,
                        Parallel = true,
                        ParallelThread = 5,
                        ComicSaver = new DelegateComicSaver(
                             x =>
                             {
                                 Total = x.Total;
                                 Current = x.Current;
                                 DownloadTip = $"解析{x.ComicVisitor.Chapter.Title}";
                                 return Task.CompletedTask;
                             },
                             x =>
                             {
                                 Total = x.Total;
                                 Current = x.Current;
                                 DownloadTip = "下载中";
                                 return Task.CompletedTask;
                             })
                    });
                }
                finally
                {
                    Total = Current = 0;
                    DownloadVisibility = Visibility.Collapsed;
                }
            });

            SwitchCommandVisibilitCommand = new RelayCommand(() =>
              {
                  CommandVisibility = CommandVisibility == Visibility.Visible ?
                    Visibility.Collapsed : Visibility.Visible;
              });
            _ = WpfComicVisitor.FirstChapterAsync().ContinueWith(_ => WpfComicVisitor.ToPageAsync(0));
            Title = $"正在阅读 - {WpfComicVisitor.Comic.Name}";
        }
        public WpfComicWatcher WpfComicVisitor { get; }
        private readonly IServiceScope scope;
        private Visibility commandVisibility = Visibility.Collapsed;
        private Visibility downloadVisibility = Visibility.Collapsed;
        private string downloadTip;
        private string keyword;
        private double scaleX = 1;
        private double scaleY = 1;
        private BitmapScalingMode scalingMode;
        private Visibility instructionsVisibility;
        private string title;
        private int total;
        private int current;

        public int Current
        {
            get { return current; }
            set => Set(ref current, value);
        }

        public int Total
        {
            get { return total; }
            set => Set(ref total, value);
        }

        public string Title
        {
            get { return title; }
            set => Set(ref title, value);
        }

        public Visibility InstructionsVisibility
        {
            get { return instructionsVisibility; }
            set => Set(ref instructionsVisibility, value);
        }

        public BitmapScalingMode ScalingMode
        {
            get { return scalingMode; }
            set => Set(ref scalingMode, value);
        }

        public double ScaleY
        {
            get { return scaleY; }
            set => Set(ref scaleY, value);
        }

        public double ScaleX
        {
            get { return scaleX; }
            set => Set(ref scaleX, value);
        }

        public string Keyword
        {
            get { return keyword; }
            set => Set(ref keyword, value);
        }

        public string DownloadTip
        {
            get { return downloadTip; }
            set => Set(ref downloadTip, value);
        }

        public Visibility DownloadVisibility
        {
            get { return downloadVisibility; }
            set => Set(ref downloadVisibility, value);
        }

        public Visibility CommandVisibility
        {
            get { return commandVisibility; }
            set => Set(ref commandVisibility, value);
        }
        public ICommand DownloadAllCommand { get; }
        public ICommand ZoomAddCommand { get; }
        public ICommand ZoomSubCommand { get; }
        public ICommand SetScalingModeCommand { get; }
        public ICommand ShowInstructionsCommand { get; }
        public ICommand HideInstructionsCommand { get; }

        public ICommand SwitchCommandVisibilitCommand { get; }

        public void HideInstructions()
        {
            InstructionsVisibility = Visibility.Collapsed;
        }
        public void ShowInstructions()
        {
            InstructionsVisibility = Visibility.Visible;
        }

        public void ZoomAdd()
        {
            ScaleX += 0.25;
            ScaleY += 0.25;
        }
        public void ZoomSub()
        {
            ScaleX -= 0.25;
            ScaleY -= 0.25;
        }

        public void SetScalingMode(BitmapScalingMode mode)
        {
            ScalingMode = mode;
        }
        public void Dispose()
        {
            scope.Dispose();
        }
    }
}
