using Anf.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Services
{
    internal class PlatformService : IPlatformService
    {
        public void Copy(string text)
        {
            App.Current.Clipboard.SetTextAsync(text).GetAwaiter().GetResult();
        }

        public Task OpenAddressAsync(string address)
        {
#if NETCOREAPP3_0_OR_GREATER
            var psi = new ProcessStartInfo
	        {
    	            FileName = address,
    	            UseShellExecute = true
	        };
	        Process.Start(psi);
#else
            Process.Start(address);
#endif
            return Task.CompletedTask;
        }
    }
}
