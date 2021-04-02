using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Kw.Comic.Avalon.ViewModels;

namespace Kw.Comic.Avalon.Views
{
    public class HomePage : UserControl
    {
        private ZoomBorder _zoomBorder;
        private readonly AvalonHomeViewModel vm=new AvalonHomeViewModel();
        public HomePage()
        {
            InitializeComponent();
            DataContext = vm;
        }
        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            vm.Dispose();
            base.OnDetachedFromLogicalTree(e);
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            _zoomBorder = this.Find<ZoomBorder>("ZoomBorder");
            if (_zoomBorder != null)
            {
                _zoomBorder.KeyDown += ZoomBorder_KeyDown;

                _zoomBorder.ZoomChanged += ZoomBorder_ZoomChanged;
            }
        }
        private void ZoomBorder_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F:
                    _zoomBorder?.Fill();
                    break;
                case Key.U:
                    _zoomBorder?.Uniform();
                    break;
                case Key.R:
                    _zoomBorder?.ResetMatrix();
                    break;
                case Key.T:
                    _zoomBorder?.ToggleStretchMode();
                    _zoomBorder?.AutoFit();
                    break;
            }
        }

        private void ZoomBorder_ZoomChanged(object sender, ZoomChangedEventArgs e)
        {
            //Debug.WriteLine($"[ZoomChanged] {e.ZoomX} {e.ZoomY} {e.OffsetX} {e.OffsetY}");
        }
    }
}
