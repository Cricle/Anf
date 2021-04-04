using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace Kw.Comic.Avalon.Services
{
    internal class ViewActiver : Dictionary<Type, Func<IControl>>, IViewActiver
    {
        public IControl Active(Type type)
        {
            return this[type]();
        }
    }
}
