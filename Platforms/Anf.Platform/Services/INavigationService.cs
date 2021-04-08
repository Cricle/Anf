using Anf.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Services
{
    public interface IComicTurnPageService
    {
        void GoSource(ComicSourceInfo info);
        void GoSource(string info);
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
