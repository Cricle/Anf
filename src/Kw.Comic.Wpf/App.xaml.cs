using Kw.Comic.Engine.Easy.Visiting;
using Kw.Comic.Wpf.Managers;
using Kw.Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Kw.Comic.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppEngine.AddDefaultsServices(AppDomain.CurrentDomain.BaseDirectory);
            AppEngine.Services.AddEasyComic();
            AppEngine.Services.AddSingleton<ICommandManager, CommandManager>();
            AppEngine.Services.AddSingleton<CommandBarManager>();
            AppEngine.Services.AddSingleton(new MainNavigationService());
            AppEngine.Services.AddSingleton(new RecyclableMemoryStreamManager());
            AppEngine.Services.AddScoped<IComicVisiting<ImageSource>, WpfComicVisiting>();
            AppEngine.Services.AddSingleton<IResourceFactoryCreator<ImageSource>, WpfResourceFactoryCreator>();

            base.OnStartup(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject?.ToString());
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            if (e.Exception?.InnerExceptions != null && e.Exception?.InnerExceptions.Count > 0)
            {
                MessageBox.Show(e.Exception.InnerExceptions[0].Message);
            }
            else
            {
                MessageBox.Show(e.Exception.Message);
            }
            e.SetObserved();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception?.Message);
            e.Handled = true;
        }
    }
}
