using Anf.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Anf.Phone.Services
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
