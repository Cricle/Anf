using Avalonia.Controls;
using System;

namespace Anf.Avalon.Services
{
    internal class NotifyToken: INotifyToken
    {
        private readonly NotifyService notifyService;

        public NotifyToken(NotifyService notifyService, IControl control)
        {
            this.notifyService = notifyService;
            Control = control;
        }

        public IControl Control { get; }
        
        public bool IsRemoved => !notifyService.Contains(Control);

        public event Action<INotifyToken> Removed;

        public void Remove()
        {
            var ok= notifyService.Remove(Control);
            if (ok)
            {
                Removed?.Invoke(this);
            }
        }
    }
}
