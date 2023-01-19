using Anf.Platform.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform
{
    public static class GlobalExceptionHelper
    {
        private static bool enable;

        public static bool Enable
        {
            get => enable;
            set
            {
                if (enable==value)
                {
                    return;
                }
                if (value)
                {
                    AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
                    TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
                }
                else
                {
                    AppDomain.CurrentDomain.UnhandledException -= OnCurrentDomainUnhandledException;
                    TaskScheduler.UnobservedTaskException -= OnTaskSchedulerUnobservedTaskException;
                }
                enable = value;
            }
        }

        private static void OnTaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var exser = AppEngine.GetRequiredService<ExceptionService>();
            exser.Exception = e.Exception;
            var logger = AppEngine.GetLogger<TaskScheduler>();
            logger.LogError(default,e.Exception, sender?.ToString() ?? string.Empty);
            e.SetObserved();
        }

        private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exser = AppEngine.GetRequiredService<ExceptionService>();
            var ex = e.ExceptionObject as Exception;
            var logger = AppEngine.GetLogger<AppDomain>();
            logger.LogError(default,ex, sender?.ToString() ?? string.Empty);
            exser.Exception = ex;
        }

    }
}
