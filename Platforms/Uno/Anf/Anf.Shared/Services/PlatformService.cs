using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace Anf.Services
{
    internal class PlatformService : IPlatformService
    {
        public void Copy(string text)
        {
            var dp = new DataPackage();
            dp.SetText(text);
            Clipboard.SetContent(dp);
        }

        public Task OpenAddressAsync(string address)
        {
#if !HAS_UNO_SKIA_WPF
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
