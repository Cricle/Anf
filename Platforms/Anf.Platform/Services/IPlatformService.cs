using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Services
{
    public interface IPlatformService
    {
        void Copy(string text);

        Task OpenAddressAsync(string address);
    }
}
