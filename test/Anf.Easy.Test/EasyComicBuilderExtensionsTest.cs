using Anf.Easy.Visiting;
using Anf.Networks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Anf.Easy.Test
{
    [TestClass]
    public class EasyComicBuilderExtensionsTest
    {
        [TestMethod]
        public void GivenNullValue_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => EasyComicBuilderExtensions.AddEasyComic(null, NetworkAdapterTypes.HttpClient));
            Assert.ThrowsException<ArgumentNullException>(() => EasyComicBuilderExtensions.AddDefaultEasyComic(null, NetworkAdapterTypes.HttpClient));
            Assert.ThrowsException<ArgumentNullException>(() => EasyComicBuilderExtensions.AddStreamVisitor(null));
        }
        [TestMethod]
        public void CallAddStreamVisitor_ServiceCollectionMustContainesServices()
        {
            var services = new ServiceCollection();
            EasyComicBuilderExtensions.AddStreamVisitor(services);
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(IResourceFactoryCreator<Stream>)));
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(IComicVisiting<Stream>)));
        }
        [TestMethod]
        [DataRow(NetworkAdapterTypes.HttpClient)]
        [DataRow(NetworkAdapterTypes.WebRequest)]
        public void CallAddEasyComic_ServiceCollectionMustContainesServices(NetworkAdapterTypes type)
        {
            var services = new ServiceCollection();
            EasyComicBuilderExtensions.AddEasyComic(services, type);
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(ComicEngine)));
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(SearchEngine)));
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(RecyclableMemoryStreamManager)));
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(INetworkAdapter)));
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(IComicDownloader)));
        }
        [TestMethod]
        [DataRow(NetworkAdapterTypes.HttpClient)]
        [DataRow(NetworkAdapterTypes.WebRequest)]
        public void CallAddDefaultEasyComic(NetworkAdapterTypes type)
        {
            var services = new ServiceCollection();
            EasyComicBuilderExtensions.AddDefaultEasyComic(services, type);
            var se = services.BuildServiceProvider()
                .GetRequiredService<SearchEngine>();
            Assert.IsNotNull(se);
        }
    }
}
