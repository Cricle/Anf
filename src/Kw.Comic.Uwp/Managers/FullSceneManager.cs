using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Kw.Comic.Uwp.Managers
{
    public class FullSceneManager
    {
        private readonly Stack<UIElement> elements = new Stack<UIElement>();

        public int Count => elements.Count;

        public event Action<UIElement> Pushed;

        public event Action<UIElement> Poped;

        public void Push(UIElement element)
        {
            elements.Push(element);
            Pushed?.Invoke(element);
        }

        public bool TryPop(out UIElement element)
        {
            var val= elements.TryPop(out element);
            if (val)
            {
                Poped?.Invoke(element);
            }
            return val;
        }

        public void Clear()
        {
            while (Count!=0)
            {
                TryPop(out _);
            }
        }
    }
}
