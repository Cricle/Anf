using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace Kw.Comic.Avalon.Services
{
    public interface IViewActiver : IDictionary<Type, Func<IControl>>
    {
        IControl Active(Type type);
    }
}
