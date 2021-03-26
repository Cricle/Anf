using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kw.Comic
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            AppEngine.Reset();
            AppEngine.AddServices();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
