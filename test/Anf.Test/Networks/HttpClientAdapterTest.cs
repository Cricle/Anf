using Anf.Networks;
using Anf.Test.Resource;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Test.Networks
{
    [TestClass]
    public class HttpClientAdapterTest
    {
        [TestMethod]
        public void InitWithNullValue_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new HttpClientAdapter(null));
        }
        [TestMethod]
        public void InitWithNotNullValue_MustCreated()
        {
            var client = new HttpClient();
            var ada=new HttpClientAdapter(client);
            Assert.AreEqual(ada.HttpClient, client);
        }
        [TestMethod]
        public void SendWithNullParams_MustThrowException()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async() =>await new HttpClientAdapter(new HttpClient()).GetStreamAsync(null));
        }
        [TestMethod]
        [DataRow("POST")]
        [DataRow("GET")]
        [DataRow("PUT")]
        public async Task SendWithHeaders_MustWithHeadersSend(string method)
        {
            var ada = new HttpClientAdapter(new HttpClient());
            using (var ser = new EngineService())
            {
                var url = "http://localhost:8765/";
                ser.Listener.Prefixes.Add(url);
                ser.Listen();
                var req = new RequestSettings
                {
                    Address = url,
                    Accept = "application/json",
                    Headers = new Dictionary<string, string>
                    {
                        ["A"] = "a",
                        ["B"] = "b"
                    },
                    Host = "www.localhost.com",
                    Method = method,
                    Referrer = "http://www.referrer.com",
                    Timeout = 1000
                };
                var rep = await ada.GetMessageAsync(req);
                if (!string.Equals(rep.RequestMessage.Method.Method, req.Method, StringComparison.InvariantCulture))
                {
                    Assert.Fail();
                }
                Assert.AreEqual(req.Host, rep.RequestMessage.Headers.GetValues("host").First());
                Assert.AreEqual(req.Headers["A"], rep.RequestMessage.Headers.GetValues("A").First());
                Assert.AreEqual(req.Headers["B"], rep.RequestMessage.Headers.GetValues("B").First());
                Assert.AreEqual(req.Accept, rep.RequestMessage.Headers.GetValues("Accept").First());
            }
        }
    }
}
