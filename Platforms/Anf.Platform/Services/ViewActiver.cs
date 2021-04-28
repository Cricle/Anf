using System;
using System.Collections.Generic;

namespace Anf.Platform.Services
{
    public class ViewActiver<TView> : Dictionary<Type, Func<TView>>, IViewActiver<TView>
    {
        public TView Active(Type type)
        {
            return this[type]();
        }
    }
}
