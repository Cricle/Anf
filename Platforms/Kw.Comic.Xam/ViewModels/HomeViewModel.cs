using GalaSoft.MvvmLight;
using Kw.Comic.Engine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kw.Comic.ViewModels
{
    public class HomeViewModel : ViewModelBase,IDisposable
    {
        public const int PageSize = 50;

        private IComicCursor comicCursor;
        private string keyword;
        private bool emptySet;

        public HomeViewModel(SearchEngine searchEngine)
        {
            SearchEngine = searchEngine;
            Snapshots = new ObservableCollection<ComicSnapshot>();
            MoveNextCommand = new Command(() => _ = MoveNextAsync());
            SearchCommand = new Command(() => _ = SearchAsync());
        }

        public bool EmptySet
        {
            get { return emptySet; }
            set => Set(ref emptySet, value);
        }

        public string Keyword
        {
            get { return keyword; }
            set => Set(ref keyword, value);
        }


        public SearchEngine SearchEngine { get; }

        public Command SearchCommand { get; }
        public Command MoveNextCommand { get; }

        public ObservableCollection<ComicSnapshot> Snapshots { get; }

        public async Task SearchAsync()
        {
            comicCursor?.Dispose();
            Snapshots.Clear();
            comicCursor = await SearchEngine.GetSearchCursorAsync(Keyword, 0, PageSize);
            InsertDatas();
        }
        private void InsertDatas()
        {
            var sn = comicCursor?.Current?.Snapshots;
            if (sn != null)
            {
                foreach (var item in sn)
                {
                    Snapshots.Add(item);
                }
            }
        }
        public async Task MoveNextAsync()
        {
            await comicCursor.MoveNextAsync();
            InsertDatas();
        }

        public void Dispose()
        {
            comicCursor?.Dispose();
        }
    }

}
