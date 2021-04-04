using Avalonia.Controls;
using System;

namespace Kw.Comic.Avalon.Services
{
    public interface INotifyToken
    {
        IControl Control { get; }
        bool IsRemoved { get; }
        event Action<INotifyToken> Removed;
        void Remove();
    }
}
