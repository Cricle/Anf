using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace Anf.Avalon.Services
{
    public interface IViewActiver : IDictionary<Type, Func<IControl>>
    {
        IControl Active(Type type);
    }
}
