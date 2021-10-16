using Anf.Desktop.Settings;
using Anf.Easy.Visiting;
using Anf.Platform;
using Anf.Platform.Services;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Services
{
    internal class DesktopStoreComicVisiting : StoreComicVisiting<Bitmap>
    {
        public DesktopStoreComicVisiting(IServiceProvider host, 
            IResourceFactoryCreator<Bitmap> resourceFactoryCreator,
            AnfSettings settings) 
            : base(host, resourceFactoryCreator)
        {
            anfSettings = settings;
            EnableRemote = false;
        }
        private readonly AnfSettings anfSettings;
        public override bool UseStore { get => anfSettings.Performace.UseStore; set => anfSettings.Performace.UseStore = value; }
        public override bool EnableRemote { get => anfSettings.Performace.EnableRemoteFetch; set => anfSettings.Performace.EnableRemoteFetch = value; }
        
    }
}
