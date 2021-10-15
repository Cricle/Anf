using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Anf.Platform.Services
{
    public class DefaultObservableCollectionFactory : IObservableCollectionFactory
    {
        public void AddRange<T>(IList<T> lst, IEnumerable<T> datas)
        {
            if (lst is null)
            {
                throw new ArgumentNullException(nameof(lst));
            }

            if (datas is null)
            {
                throw new ArgumentNullException(nameof(datas));
            }

            if (lst is SilentObservableCollection<T> coll)
            {
                coll.AddRange(datas);
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
            return new SilentObservableCollection<T>();
        }

        public IList<T> Create<T>(IEnumerable<T> datas)
        {
            if (datas is null)
            {
                throw new ArgumentNullException(nameof(datas));
            }

            return new SilentObservableCollection<T>(datas);
        }
    }
    public interface IObservableCollectionFactory
    {
        IList<T> Create<T>();

        IList<T> Create<T>(IEnumerable<T> datas);

        void AddRange<T>(IList<T> lst, IEnumerable<T> datas);
    }
}
