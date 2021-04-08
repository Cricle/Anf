using Avalonia.Controls;
using System;

namespace Anf.Avalon.Services
{
    public interface INotifyToken
    {
        IControl Control { get; }
        bool IsRemoved { get; }
        event Action<INotifyToken> Removed;
        void Remove();
    }
}
