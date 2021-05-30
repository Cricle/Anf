using Anf.Services;
using Microsoft.Maui.Essentials;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Cross.Services
{
    internal class PlatformService : IPlatformService
    {
        public void Copy(string text)
        {
            Clipboard.SetTextAsync(text).GetAwaiter().GetResult();
        }

        public Task OpenAddressAsync(string address)
        {
            return Browser.OpenAsync(address);
        }
    }
}
