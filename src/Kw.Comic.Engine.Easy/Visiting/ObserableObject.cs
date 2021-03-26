using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public abstract class ObserableObject : INotifyPropertyChanged
    {
        protected void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        protected void RaisePropertyChanged([CallerMemberName]string name=null)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(name));
        }
        protected void RaisePropertyChanged<T>(ref T prop,T value,[CallerMemberName] string name = null)
        {
            if (!EqualityComparer<T>.Default.Equals(prop, value))
            {
                prop = value;
                RaisePropertyChanged(new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
