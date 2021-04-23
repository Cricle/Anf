using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Platform
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class ConcurrentOnce
    {
        private object token = new object();
        public object Token => token;

        public async Task<bool> WaitAsync(TimeSpan delayTime)
        {
            var tk = token;
            await Task.Delay(delayTime);
            return Interlocked.CompareExchange(ref token, new object(), tk) == tk;
        }

        public override string ToString()
        {
            return $"{{{Token}}}";
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
