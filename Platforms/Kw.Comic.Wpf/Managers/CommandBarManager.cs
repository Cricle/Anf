using Kw.Core.Annotations;
using Kw.Visio.Selectors;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf.Managers
{
    [EnableService(ServiceLifetime = ServiceLifetime.Singleton)]
    public class CommandBarManager
    {
        public CommandBarManager()
        {
            LeftCommands = new ObservableCollection<object>();
            RightCommands = new ObservableCollection<object>();
        }

        public ObservableCollection<object> LeftCommands { get; }

        public ObservableCollection<object> RightCommands { get; }
    }
}
