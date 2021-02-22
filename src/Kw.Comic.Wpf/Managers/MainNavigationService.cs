using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Kw.Comic.Wpf.Managers
{
    public class MainNavigationService
    {
        public Frame Frame { get; } = new Frame();
    }
}
