using Avalonia.Controls;
using GalaSoft.MvvmLight;
using Kw.Comic.Avalon.ViewModels;
using Kw.Comic.Avalon.Views;
using Kw.Comic.Models;
using Kw.Comic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon.Services
{
    internal class MainNavigationService : ObservableObject,INavigationService,IComicTurnPageService
    {
        private readonly IViewActiver viewActiver;
        private readonly Stack<Type> types;
        internal readonly Border border;
        public MainNavigationService(Border border, IViewActiver viewActiver)
        {
            this.border = border;
            this.viewActiver = viewActiver;
            types = new Stack<Type>();
        }

        public bool CanGoBack => types.Count != 0;

        public bool CanGoForward => false;

        public bool GoBack()
        {
            if (CanGoBack)
            {
                var type = types.Pop();
                var control=viewActiver.Active(type);
                NavigateCore(control);
            }
            return false;
        }

        public bool GoForward()
        {
            throw new NotSupportedException();
        }

        public void GoSource(ComicSourceInfo info)
        {
            //Todo
            var view = new VisitingView(info.Source.TargetUrl);
            Navigate(view);
        }
        private IControl NavigateCore(object dest)
        {
            if (dest is IControl control)
            {
                border.Child = control;
            }
            else
            {
                border.Child = new TextBlock { Text = dest?.ToString() };
            }
            return border.Child;
        }
        public IControl Navigate(object dest)
        {
            var c = NavigateCore(dest);
            types.Push(c.GetType());
            return c;
        }
        public IControl Navigate(Type type)
        {
            var control = viewActiver[type]();
            return Navigate(control);
        }
        public IControl Navigate<T>()
        {
            return Navigate(typeof(T));
        }

        void INavigationService.Navigate(object dest)
        {
            this.Navigate(dest);
        }
    }
}
