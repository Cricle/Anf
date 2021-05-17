using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform
{
    internal static class TaskHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
