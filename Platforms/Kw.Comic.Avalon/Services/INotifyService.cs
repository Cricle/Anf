using Avalonia.Controls;
using System.Collections.Generic;

namespace Kw.Comic.Avalon.Services
{
    public interface INotifyService:IReadOnlyList<IControl>
    {
        INotifyToken Create(IControl control);

        bool ContainsControl(IControl control);

        void ClearAll();
    }
}
