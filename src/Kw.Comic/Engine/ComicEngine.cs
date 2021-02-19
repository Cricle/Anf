using Ao.Lang;
using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kw.Comic.Engine
{
    [EnableService(ServiceLifetime = ServiceLifetime.Singleton)]
    public class ComicEngine : ObservableCollection<IComicSourceCondition>
    {
        public ComicEngine()
        {
            CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action== NotifyCollectionChangedAction.Add||e.Action== NotifyCollectionChangedAction.Remove)
            {
                this.SortDescending(x => x.Order);
            }
        }
        private IComicSourceCondition CoreGetComicSourceProviderType(ComicSourceContext ctx)
        {
            foreach (var item in this)
            {
                if (item.Condition(ctx))
                {
                    return item;
                }
            }
            return null;
        }
        public IComicSourceCondition GetComicSourceProviderType(string targetUri)
        {
            var ctx = new ComicSourceContext(targetUri);
            return CoreGetComicSourceProviderType(ctx);
        }
        public IComicSourceCondition GetComicSourceProviderType(Uri targetUri)
        {
            var ctx = new ComicSourceContext(targetUri);
            return CoreGetComicSourceProviderType(ctx);
        }
    }
}
