using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Anf.Services
{
    internal class DependencyAppService : ObservableObject
    {
        private Application app;
        private Window window;

        public Application App
        {
            get
            {
                if (app == null)
                {
                    app = Application.Current;
                }
                return app;
            }
        }

        public Window Window
        {
            get
            {
                if (window == null)
                {
                    window = Window.Current;
                }
                return window;
            }
        }
        protected void RunOnUI(DispatchedHandler handler)
        {
            RunOnUIAsync(handler).GetAwaiter().GetResult();
        }
        protected Task RunOnUIAsync(DispatchedHandler handler)
        {
            return RunOnUIAsync(handler, CoreDispatcherPriority.Normal);
        }
        protected void RunOnUI(DispatchedHandler handler, CoreDispatcherPriority priority)
        {
            RunOnUIAsync(handler, priority).GetAwaiter().GetResult();
        }
        protected async Task RunOnUIAsync(DispatchedHandler handler, CoreDispatcherPriority priority)
        {
            var win = Window;
            if (win.Dispatcher.HasThreadAccess)
            {
                handler();
            }
            else
            {
                await win.Dispatcher.RunAsync(priority, handler);
            }
        }
    }
}
