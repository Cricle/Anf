using Anf.Models;
using Anf.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

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
        private Type oldPageType;
        private object oldParamter;
        public void Navigate(Type sourcePageType, object paramter)
        {
            if (oldPageType != null)
            {
                runtime.ContentFrame.BackStack
                    .Add(new PageStackEntry(oldPageType, oldParamter, new SlideNavigationTransitionInfo()));
            }
            runtime.ContentFrame.Navigate(sourcePageType, paramter);
            oldPageType = sourcePageType;
            oldParamter = paramter;
        }
        public void GoSource(ComicSourceInfo info)
        {
            GoSource(info.Source.TargetUrl);
        }
        public void GoSource(string address)
        {
            Navigate(typeof(VisitingView), address);
        }
    }
}
