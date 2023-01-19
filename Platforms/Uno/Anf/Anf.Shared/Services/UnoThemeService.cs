using Windows.UI.Xaml;

namespace Anf.Services
{
    internal class UnoThemeService: DependencyAppService
    {
        public UnoThemeService()
        {

        }

        public ApplicationTheme Theme
        {
            get => App.RequestedTheme;
            set
            {
                RunOnUI(() => App.RequestedTheme = value);
            }
        }


    }
}
