using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Anf.Settings
{
    public class WindowSettings : ObservableObject
    {
        private double minWidth = 650;
        private double minHeight = 400;
        private bool topmost;

        public virtual bool Topmost
        {
            get { return topmost; }
            set => SetProperty(ref topmost, value);
        }

        public virtual double MinHeight
        {
            get { return minHeight; }
            set => SetProperty(ref minHeight, value);
        }

        public virtual double MinWidth
        {
            get { return minWidth; }
            set => SetProperty(ref minWidth, value);
        }

    }
}
