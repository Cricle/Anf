using Avalonia.Controls;
using GalaSoft.MvvmLight;
using Anf.Avalon.ViewModels;
using Anf.Avalon.Views;
using Anf.Models;
using Anf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Avalon.Services
{
    internal class MainNavigationService : ObservableObject, INavigationService, IComicTurnPageService
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

        public bool CanGoBack => types.Count !=0;

        public bool CanGoForward => false;

        public bool GoBack()
        {
            if (CanGoBack)
            {
                var t=types.Pop();
                if (CanGoBack && border.Child.GetType().IsEquivalentTo(t))
                {
                    t = types.Pop();
                }
                var control=viewActiver.Active(t);
                NavigateCore(control);
                if (types.Count==0)
                {
                    types.Push(t);
                }
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
            GoSource(info.Source.TargetUrl);
        }
        public void GoSource(string address)
        {
            //Todo
            var view = new VisitingView(address);
            Navigate(view);
        }
        public Type BoderChildType => border.Child?.GetType();
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
            var originType = BoderChildType;
            if (originType!=null&&BoderChildType.IsEquivalentTo(type))
            {
                return border.Child;
            }
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
