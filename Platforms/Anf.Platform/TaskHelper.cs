using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform
{
    internal static class TaskHelper
    {
        public static Task GetComplatedTask()
        {
#if NET452 || NETSTANDARD1_4
            return Task.FromResult(false);
#else
            return Task.CompletedTask;
#endif
        }
    }
}
