using System;
using System.Collections.Generic;

namespace Anf.Platform.Services
{
    public interface IViewActiver<TView> : IDictionary<Type, Func<TView>>
    {
        TView Active(Type type);
    }
}
