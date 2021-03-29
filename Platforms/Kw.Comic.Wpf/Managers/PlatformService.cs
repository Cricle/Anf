using Kw.Comic.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kw.Comic.Wpf.Managers
{
    internal class PlatformService : IPlatformService
    {
        public void Copy(string text)
        {
            Clipboard.SetText(text);
        }

        public Task OpenAddressAsync(string address)
        {
            Process.Start(address);
            return Task.CompletedTask;
        }
    }
}
