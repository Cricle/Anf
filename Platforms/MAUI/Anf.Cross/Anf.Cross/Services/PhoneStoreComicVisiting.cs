using Anf.Easy.Visiting;
using Anf.Cross.Settings;
using Anf.Platform;
using Anf.Platform.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Controls;

namespace Anf.Cross.Services
{
    public class PhoneStoreComicVisiting : StoreComicVisiting<ImageSource>
    {
        public PhoneStoreComicVisiting(IServiceProvider host, IResourceFactoryCreator<ImageSource> resourceFactoryCreator) : base(host, resourceFactoryCreator)
        {
            anfSettings = AppEngine.GetRequiredService<AnfSettings>();
        }
        private readonly AnfSettings anfSettings;
        public override bool UseStore { get => anfSettings.Performace.UseStore; set => anfSettings.Performace.UseStore = value; }
        public override bool EnableRemote { get => anfSettings.Performace.EnableRemoteFetch; set => anfSettings.Performace.EnableRemoteFetch = value; }
    }
}
