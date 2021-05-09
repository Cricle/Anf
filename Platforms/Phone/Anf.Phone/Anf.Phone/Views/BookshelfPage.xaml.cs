using Anf.Phone.ViewModels;
using Anf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Anf.Phone.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookshelfPage : ContentPage
    {
        public BookshelfPage()
        {
            InitializeComponent();
            BindingContext = new PhoneBookshelfViewModel();
        }
    }
}