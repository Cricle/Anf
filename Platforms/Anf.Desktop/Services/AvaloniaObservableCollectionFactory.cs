using Anf.Platform.Services;
using Avalonia.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Services
{
    internal class AvaloniaObservableCollectionFactory : IObservableCollectionFactory
    {
        public void AddRange<T>(IList<T> lst, IEnumerable<T> datas)
        {
            if (lst is AvaloniaList<T> l)
            {
                l.AddRange(datas);
            }
            else
            {
                foreach (var item in datas)
                {
                    lst.Add(item);
                }
            }
        }

        public IList<T> Create<T>()
        {
            return new AvaloniaList<T>();
        }

        public IList<T> Create<T>(IEnumerable<T> datas)
        {
            return new AvaloniaList<T>(datas);
        }
    }
}
