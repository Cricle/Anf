using GalaSoft.MvvmLight;
using Kw.Comic.Engine.Easy.Store;
using Kw.Comic.Models;
using Kw.Comic.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.ViewModels
{
    public class BookshelfViewModel : ViewModelBase
    {
        public const int PageSize = 10;

        private readonly IBookshelfService bookshelfService;

        private int currentPage;
        private int totalPage;
        private long total;
        private bool hasNextPage;
        private bool hasPrevPage;
        private bool isEmpty;
        private string keyword;
        private bool searching;

        public BookshelfViewModel(IBookshelfService bookshelfService)
        {
            this.bookshelfService = bookshelfService;
        }

        public bool Searching
        {
            get { return searching; }
            private set => Set(ref searching, value);
        }

        public string Keyword
        {
            get { return keyword; }
            set => Set(ref keyword, value);
        }

        public bool IsEmpty
        {
            get { return isEmpty; }
            private set => Set(ref isEmpty, value);
        }

        public bool HasPrevPage
        {
            get { return hasPrevPage; }
            private set => Set(ref hasPrevPage, value);
        }

        public bool HasNextPage
        {
            get { return hasNextPage; }
            private set => Set(ref hasNextPage, value);
        }

        public long Total
        {
            get { return total; }
            private set => Set(ref total, value);
        }

        public int TotalPage
        {
            get { return totalPage; }
            private set => Set(ref totalPage, value);
        }

        public int CurrentPage
        {
            get { return currentPage; }
            private set => Set(ref currentPage, value);
        }


        public ObservableCollection<Bookshelf> Bookshelves { get; }

        public async Task LoadBookshelfAsync(bool clearPrev)
        {
            Searching = true;
            try
            {
                if (clearPrev)
                {
                    Bookshelves.Clear();
                }
                var datas = await bookshelfService.FindBookShelfAsync(CurrentPage * PageSize, PageSize);
                Total = datas.Total;
                foreach (var item in datas.Datas)
                {
                    Bookshelves.Add(item);
                }
                UpdateInfo();
            }
            finally
            {
                Searching = false;
            }
        }
        public Task NextPageAsync(bool clearPrev)
        {
            CurrentPage++;
            return LoadBookshelfAsync(clearPrev);
        }
        private void UpdateInfo()
        {
            HasPrevPage = Total != 0 && CurrentPage != 0;
            TotalPage = (int)Math.Ceiling((double)(Total) / PageSize);
            HasNextPage = Total != 0 && TotalPage > CurrentPage;
            IsEmpty = Total == 0;
        }
    }
}
