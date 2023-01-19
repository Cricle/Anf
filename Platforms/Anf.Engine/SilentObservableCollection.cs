using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace System.Collections.ObjectModel
{
	/// <summary>
	/// SilentObservableCollection is a ObservableCollection with some extensions.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SilentObservableCollection<T> : ObservableCollection<T>
	{
        public SilentObservableCollection()
        {
        }

        public SilentObservableCollection(IEnumerable<T> collection) : base(collection)
        {
        }

        public SilentObservableCollection(List<T> list) : base(list)
        {
        }

        internal static class EventArgsCache
		{
			internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");
			internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new PropertyChangedEventArgs("Item[]");
			internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
		}
		/// <summary>
		/// Adds a range of items to the observable collection.
		/// Instead of iterating through all elements and adding them
		/// one by one (which causes OnPropertyChanged events), all
		/// the items gets added instantly without firing events.
		/// After adding all elements, the OnPropertyChanged event will be fired.
		/// </summary>
		/// <param name="enumerable"></param>
		public void AddRange(IEnumerable<T> enumerable)
		{
			CheckReentrancy();

			int startIndex = Count;

			foreach (var item in enumerable)
				Items.Add(item);

			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(enumerable), startIndex));
			OnPropertyChanged(EventArgsCache.CountPropertyChanged);
			OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
		}
	}
}
