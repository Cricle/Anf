using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform
{
    internal static class TaskHelper
    {
#if NET452 || NETSTANDARD1_4
        private static readonly Task ComplatedTask = Task.FromResult(false);
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task GetComplatedTask()
        {
#if NET452 || NETSTANDARD1_4
            return ComplatedTask;
#else
            return Task.CompletedTask;
#endif
        }
    }
}
