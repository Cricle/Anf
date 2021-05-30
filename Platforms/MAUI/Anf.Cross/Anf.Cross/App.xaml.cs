using Anf.Cross.ViewModels;
using Anf.Cross.Views;
using Microsoft.Maui;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Application = Microsoft.Maui.Controls.Application;

namespace Anf.Cross
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override IWindow CreateWindow(IActivationState activationState)
        {
            Microsoft.Maui.Controls.Compatibility.Forms.Init(activationState);

            this.On<Microsoft.Maui.Controls.PlatformConfiguration.Windows>()
                .SetImageDirectory("Assets");
            _ = AppEngine.Provider;
            return new Microsoft.Maui.Controls.Window(new HomePage());
        }
    }
}
