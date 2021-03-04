using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.ObjectModel
{
    internal static class ObservableCollectionExtension
    {
        public static void AddRange<T>(this ObservableCollection<T> collection,params T[] ts)
        {
            foreach (var item in ts)
            {
                collection.Add(item);
            }
        }

        public static void SortDescending<T,V>(this ObservableCollection<T> collection,Func<T,V> order)
        {
            var sortedList = collection.OrderByDescending(order).ToArray();
            for (int i = 0; i < sortedList.Length; i++)
            {
                collection.Move(collection.IndexOf(sortedList[i]), i);
            }
        }
        public static void Sort<T,V>(this ObservableCollection<T> collection, Func<T, V> order) 
        {
            var sortedList = collection.OrderBy(order).ToArray();
            for (int i = 0; i < sortedList.Length; i++)
            {
                collection.Move(collection.IndexOf(sortedList[i]), i);
            }
        }
    }
}
