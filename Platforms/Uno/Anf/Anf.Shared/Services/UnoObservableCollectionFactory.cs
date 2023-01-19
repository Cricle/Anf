using Anf.Platform.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Anf.Services
{
    internal class UnoObservableCollectionFactory : IObservableCollectionFactory
    {
        public void AddRange<T>(IList<T> lst, IEnumerable<T> datas)
        {
            foreach (var item in datas)
            {
                lst.Add(item);
            }
        }

        public IList<T> Create<T>()
        {
            return new ObservableCollection<T>();
        }

        public IList<T> Create<T>(IEnumerable<T> datas)
        {
            return new ObservableCollection<T>(datas);
        }
    }
}
