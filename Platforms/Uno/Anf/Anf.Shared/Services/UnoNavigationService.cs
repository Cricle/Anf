using Anf.Models;
using Anf.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace Anf.Services
{
    internal class UnoNavigationService : IComicTurnPageService
    {
        private readonly UnoRuntime runtime;

        public UnoNavigationService(UnoRuntime runtime)
        {
            this.runtime = runtime;
        }

        public bool CanGoBack => runtime.ContentFrame.CanGoBack;

        public bool CanGoForward => runtime.ContentFrame.CanGoForward;

        public void GoBack()
        {
            if (CanGoBack)
            {
                runtime.ContentFrame.GoBack();
            }
        }
        public void GoForward()
        {
            if (CanGoForward)
            {
                runtime.ContentFrame.GoForward();
            }
        }
        public void Navigate(Type sourcePageType,object paramter)
        {
            runtime.ContentFrame.Navigate(sourcePageType, paramter);
        }
        public void Navigate(object content)
        {
            runtime.ContentFrame.Content = content;
        }
        public void GoSource(ComicSourceInfo info)
        {
            GoSource(info.Source.TargetUrl);
        }
        public void GoSource(string address)
        {
            var view = new VisitingView(address);
            Navigate(view);
        }
    }
}
