using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon.Services
{
    internal class NotifyService: ObservableCollection<IControl>, INotifyService
    {
        readonly Dictionary<IControl, INotifyToken> tokens;
        public NotifyService()
        {
            tokens = new Dictionary<IControl, INotifyToken>();
        }
        public void ClearAll()
        {
            foreach (var item in this)
            {
                RemoveControl(item);
            }
            tokens.Clear();
            Clear();
        }
        public bool ContainsControl(IControl control)
        {
            return tokens.ContainsKey(control);
        }
        public INotifyToken Create(IControl control)
        {
            var token = new NotifyToken(this, control);
            Add(control);
            tokens[control] = token;
            return token;
        }
        internal bool RemoveControl(IControl control)
        {
            if (Remove(control))
            {
                tokens.Remove(control);
                return true;
            }
            return false;
        }
    }
}
