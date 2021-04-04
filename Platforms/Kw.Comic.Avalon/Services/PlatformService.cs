using Kw.Comic.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon.Services
{
    internal class PlatformService : IPlatformService
    {
        public void Copy(string text)
        {
            App.Current.Clipboard.SetTextAsync(text).GetAwaiter().GetResult();
        }

        public Task OpenAddressAsync(string address)
        {
            Process.Start(address);
            return Task.CompletedTask;
        }
    }
}
