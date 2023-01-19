using Anf.Cross.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;

namespace Anf.Cross.Views
{
    public partial class HomePage : ContentPage, IPage
    {
        public HomePage()
        {
            InitializeComponent();
            BindingContext = AppEngine.GetRequiredService<MAUIHomeViewModel>();
        }
    }
}
