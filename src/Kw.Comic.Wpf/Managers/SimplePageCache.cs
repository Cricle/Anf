using Kw.Comic.Visit;
using Kw.Comic.Wpf.Models;
using System;

namespace Kw.Comic.Wpf.Managers
{
    public class SimplePageCache:SimpleCacher<int, ComicPageInfo>
    {
        public SimplePageCache(int max=5)
            :base(max)
        {
        }
        protected override async void OnSwitch(int key, ComicPageInfo value)
        {
            if (SwitchDisposable)
            {
                await value.UnLoadAsync();
            }
        }
    }
}
