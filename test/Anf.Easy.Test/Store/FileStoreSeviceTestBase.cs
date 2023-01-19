using Anf.Easy.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Store
{
    public class FileStoreSeviceTestBase
    {
        private string str = "howgduizidawssds\n\n\t123123@!#@";

        public FileStoreSeviceTestBase(Func<FileStoreService> service)
        {
            Service = service;
        }

        public Func<FileStoreService> Service { get; }
        private Stream MakeStream()
        {
            var mem = new MemoryStream();
            var text = Encoding.UTF8.GetBytes(str);
            mem.Write(text,0,text.Length);
            mem.Seek(0, SeekOrigin.Begin);
            return mem;
        }
        [TestMethod]
        public async Task PutSteamToSave_TheFileMustBeCreated()
        {
            string path = null;
            try
            {

                using(var ser= Service())
                using (var mem = MakeStream())
                {
                    var uri = "dgsiagiduasv";
                    path = await ser.SaveAsync(uri, mem);
                    var fn = Path.GetFileName(path);
                    var sfn = ser.GetFileName(uri);
                    Assert.AreEqual(fn, sfn);
                    var fileExist = File.Exists(path);
                    Assert.IsTrue(fileExist);
                    var fpath = await ser.GetPathAsync(uri);
                    Assert.AreEqual(path, fpath);
                    var fileText = File.ReadAllText(path);
                    Assert.AreEqual(str, fileText);
                    using (var fs = await ser.GetStreamAsync(uri))
                    using (var sr = new StreamReader(fs))
                    {
                        Assert.IsNotNull(fs);
                        var fstr = sr.ReadToEnd();
                        Assert.AreEqual(str, fstr);
                    }
                    var exists = await ser.ExistsAsync(uri);
                    Assert.IsTrue(exists);
                    exists = await ser.ExistsAsync("--dsaksda");
                    Assert.IsFalse(exists);
                    var need = ser.NeedToSave(new ComicDownloadContext(
                        null, null, new ComicPage { TargetUrl = uri }, null, CancellationToken.None));
                    Assert.IsFalse(need);
                    need = ser.NeedToSave(new ComicDownloadContext(
                        null, null, new ComicPage { TargetUrl = "--dksjbvakjfvd" }, null, CancellationToken.None));
                    Assert.IsTrue(need);
                }
            }
            finally
            {
                if (path!=null&&File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
