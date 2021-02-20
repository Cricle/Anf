using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kw.Comic.Uwp.ViewModels
{
    public class ViewModelLocator
    {
        private readonly Lazy<MainViewModel> mainViewModel = new Lazy<MainViewModel>();

        public MainViewModel MainViewModel => mainViewModel.Value;

        public HomeViewModel HomeViewModel => UwpAppEngine.Instance.GetService<HomeViewModel>();
    }
}
