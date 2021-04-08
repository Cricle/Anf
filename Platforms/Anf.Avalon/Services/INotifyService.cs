using Avalonia.Controls;
using System.Collections.Generic;

namespace Anf.Avalon.Services
{
    public interface INotifyService:IReadOnlyList<IControl>
    {
        INotifyToken Create(IControl control);

        bool ContainsControl(IControl control);

        void ClearAll();
    }
}
