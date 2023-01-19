using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Test
{
    [TestClass]
    public class AppEngineTest
    {
        [TestMethod]
        public void ResetWithEmptyOrNot_ServicesMustDefaultOrInput()
        {
            AppEngine.Reset();
            Assert.IsNotNull(AppEngine.Services);
            var services = new ServiceCollection();
            AppEngine.Reset(services);
            Assert.AreEqual(services, AppEngine.Services);
        }
        [TestMethod]
        public void Reset_ProviderMustSetNull()
        {
            AppEngine.Reset();
            var provider = AppEngine.Provider;
            Assert.IsNotNull(provider);
            Assert.IsTrue(AppEngine.IsLoaded);
            AppEngine.Reset();
            Assert.AreNotEqual(provider,AppEngine.Provider);
        }
        [TestMethod]
        public void UseProvider_ProviderMustInput()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            AppEngine.UseProvider(provider);
            Assert.AreEqual(provider, AppEngine.Provider);
        }
        [TestMethod]
        public void AddServices_MustOk()
        {
            AppEngine.Reset();
            AppEngine.AddServices(Easy.NetworkAdapterTypes.HttpClient);
            AppEngine.Reset();
            AppEngine.AddServices(Easy.NetworkAdapterTypes.WebRequest);
            AppEngine.Reset();
        }
        [TestMethod]
        public void AddLogging_GetLoggingService_MustNotNull()
        {
            AppEngine.Reset();
            AppEngine.Services.AddLogging();
            var logger = AppEngine.GetLogger<AppEngineTest>();
            Assert.IsNotNull(logger);
        }
        [TestMethod]
        public void GetServiceScope_MustNotNull()
        {
            AppEngine.Reset();
            var scope = AppEngine.CreateScope();
            Assert.IsNotNull(scope);
            scope.Dispose();
        }
        class A { }
        class B { }
        class C { }
        [TestMethod]
        public void AddSomeServices_CallGet_MustReturnService()
        {
            AppEngine.Reset();
            AppEngine.Services.AddSingleton<A>();
            AppEngine.Services.AddScoped<B>();
            AppEngine.Services.AddTransient<C>();
            _ = AppEngine.Provider;

            Check<A>();
            Check<B>();
            Check<C>();

            void Check<T>()
            {
                var a = AppEngine.GetService(typeof(T));
                Assert.IsNotNull(a);
                Assert.IsInstanceOfType(a, typeof(T));
                a = AppEngine.GetService<T>();
                Assert.IsNotNull(a);
                a = AppEngine.GetRequiredService<T>();
                Assert.IsNotNull(a);
                a = AppEngine.GetRequiredService(typeof(T));
                Assert.IsNotNull(a);
                Assert.IsInstanceOfType(a, typeof(T));
            }

        }
    }
}
