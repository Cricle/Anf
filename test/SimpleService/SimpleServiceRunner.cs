using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleService
{
    public class SimpleServiceRunner : IDisposable
    {
        public SimpleServiceRunner(DirectoryInfo directory)
        {
            Directory = directory;
            CancellationTokenSource = new CancellationTokenSource();
            Listener = new HttpListener();
        }

        public CancellationTokenSource CancellationTokenSource { get; }

        public DirectoryInfo Directory { get; }

        public HttpListener Listener { get; }
        private async void CoreListen()
        {
            Listener.Start();
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var ctx = await Listener.GetContextAsync();
                    try
                    {
                        foreach (var item in ctx.Request.Headers.AllKeys)
                        {
                            var val = ctx.Request.Headers[item];
                            ctx.Response.Headers.Add(item, val);
                        }
                        var fp = Path.Combine(Directory.FullName, ctx.Request.Url.AbsolutePath);
                        using (var fs = File.OpenRead(fp))
                        {
                            await fs.CopyToAsync(ctx.Response.OutputStream);
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        ctx.Response.StatusCode = 404;
                    }
                    finally
                    {
                        ctx.Response.Close();
                    }
                }
                catch (OperationCanceledException)
                {

                }
            }
        }
        public void Listen()
        {
            _=Task.Run(CoreListen);
        }

        public void Dispose()
        {
            CancellationTokenSource.Cancel();
        }
    }
}
