using Kw.Comic.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kw.Comic.Services
{
    public interface IComicTurnPageService
    {
        void GoSource(ComicSourceInfo info);
    }
    public interface INavigationService
    {
        bool CanGoBack { get; }

        bool CanGoForward { get; }

        void Navigate(object dest);

        bool GoBack();

        bool GoForward();
    }
}
