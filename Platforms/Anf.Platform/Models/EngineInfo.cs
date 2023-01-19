using Anf.Services;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Models
{
    public class EngineInfo<TImage>
    {
        public EngineInfo()
        {
            OpenCommand = new RelayCommand(Open);
            CopyCommand = new RelayCommand(Copy);
        }

        public IComicSourceCondition Condition { get; set; }

        public TImage Bitmap { get; set; }

        public RelayCommand OpenCommand { get; }
        public RelayCommand CopyCommand { get; }

        public async void Open()
        {
            await AppEngine.GetRequiredService<IPlatformService>()
                .OpenAddressAsync(Condition.Address.AbsoluteUri);
        }
        public void Copy()
        {
            AppEngine.GetRequiredService<IPlatformService>()
                .Copy(Condition.Address.AbsoluteUri);
        }
    }
}
