using Kw.Comic.Engine.Easy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kw.Comic.Essentials
{
    public class AppEngine
    {
        public AppEngine()
        {
            ComicBuilder = new EasyComicBuilder();
        }

        private IComicHost comicHost;

        public EasyComicBuilder ComicBuilder { get; }

        public IComicHost ComicHost => comicHost;

        public void Init()
        {
            ComicBuilder.AddComicServices();
        }
        public void Build()
        {
            comicHost = ComicBuilder.Build();
        }
    }
}
