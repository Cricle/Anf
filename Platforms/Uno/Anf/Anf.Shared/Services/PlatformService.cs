using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

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

        public async Task OpenAddressAsync(string address)
        {
            await Launcher.LaunchUriAsync(new Uri(address));
        }
    }
}
