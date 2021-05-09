using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Anf.Phone.Models;
using Anf.Platform.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Anf.Phone.Test
{
    [TestClass]
    public class PhoneAppEngineExtensionsTest
    {
        [TestMethod]
        public void AddServices_GetAny_MustOk()
        {
            var services = new ServiceCollection();
            services.AddEasyComic();
            services.AddPhoneService();
            var provider = services.BuildServiceProvider();
            using (var scope = provider.CreateScope())
            {
                _ = scope.ServiceProvider.GetRequiredService<IComicVisiting<ImageSource>>();
                _ = scope.ServiceProvider.GetRequiredService<IStreamImageConverter<ImageSource>>();
                _ = scope.ServiceProvider.GetRequiredService<IResourceFactoryCreator<ImageSource>>();
                _ = scope.ServiceProvider.GetRequiredService<IStoreService>();
                _ = scope.ServiceProvider.GetRequiredService<IObservableCollectionFactory>();
                _ = scope.ServiceProvider.GetRequiredService<ComicStoreService<PhoneComicStoreBox>>();
            }
        }
    }
}
