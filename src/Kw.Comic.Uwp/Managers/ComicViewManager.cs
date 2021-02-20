using GalaSoft.MvvmLight;
using Kw.Comic.Engine;
using Kw.Comic.Uwp.Pages;
using Kw.Comic.Uwp.ViewModels;
using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Uwp.Managers
{
    [EnableService(ServiceLifetime = ServiceLifetime.Singleton)]
    public class ComicViewManager : ObservableObject
    {
        private readonly ComicEngine comicEngine;

        private string targetUrl;
        private IComicSourceCondition sourceCondition;

        public ComicViewManager(ComicEngine comicEngine)
        {
            this.comicEngine = comicEngine;
        }

        public IComicSourceCondition SourceCondition
        {
            get { return sourceCondition; }
            private set => Set(ref sourceCondition, value);
        }

        public string TargetUrl
        {
            get { return targetUrl; }
            set
            {
                Set(ref targetUrl, value);
                SourceCondition = null;
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        SourceCondition = comicEngine.GetComicSourceProviderType(value);
                    }
                    catch (Exception) { }
                }
            }
        }

        public async Task<ViewPage> GoAsync()
        {
            var vm =await ViewViewModel.FromUriAsync(TargetUrl);
            if (vm==null)
            {
                return null;
            }
            return new ViewPage { DataContext = vm };
        }
    }
}
