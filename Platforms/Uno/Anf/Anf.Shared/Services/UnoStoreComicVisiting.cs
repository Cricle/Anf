using Anf.Easy.Visiting;
using Anf.Platform;
using Anf.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media;

namespace Anf.Services
{
    internal class UnoStoreComicVisiting: StoreComicVisiting<ImageBox>
    {
        public UnoStoreComicVisiting(IServiceProvider host,
           IResourceFactoryCreator<ImageBox> resourceFactoryCreator,
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
