using Kw.Comic.Consolat.Commands;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Kw.Comic.Engine.Easy.Visiting;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Kw.Comic.Consolat
{
    class Program
    {
        static void Main(string[] args)
        {
            AppEngine.Reset();
            AppEngine.Services.AddDefaultEasyComic();
            Run().GetAwaiter().GetResult();
            //var app = new CommandApp();
            //app.Configure(x =>
            //{
            //    x.AddCommand<SearchCommand>("search");
            //});
            //app.Run(args);
        }
        private static async Task Run()
        {
            var visit = AppEngine.GetRequiredService<IComicVisiting<Stream>>();
            await visit.LoadAsync("https://www.kuaikanmanhua.com/web/topic/8339/");
            var chp =await visit.GetChapterManagerAsync(0);
            var page =await chp.GetVisitPageAsync(0);
            var fs = File.Open("a.png", FileMode.OpenOrCreate);
            await page.Resource.CopyToAsync(fs);
        }
    }
}
