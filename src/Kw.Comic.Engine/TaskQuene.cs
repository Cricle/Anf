using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Engine
{
    public static class TaskQuene
    {
        public static async Task<TResult[]> RunAsync<TResult>(Func<Task<TResult>>[] maxBlocks, int concurrent = 5)
        {
            var vals =await RunVoidAsync(maxBlocks, concurrent);
            return vals.OfType<Task<TResult>>().Select(x=>x.Result).ToArray();
        }
        public static async Task<Task[]> RunVoidAsync(Func<Task>[] maxBlocks, int concurrent = 5)
        {
            var tasks = new List<Task>(concurrent);
            var runningQuene = new List<Task>(concurrent);
            var pos = 0;
            while (tasks.Count < maxBlocks.Length && pos < maxBlocks.Length)
            {
                if (runningQuene.Count >= concurrent)
                {
                    var complated = Task.WhenAny(runningQuene);
                    runningQuene.Remove(complated);
                }
                var task = maxBlocks[pos]();
                tasks.Add(task);
                runningQuene.Add(task);
                pos++;
            }
            await Task.WhenAll(runningQuene);
            return tasks.ToArray();
        }
    }
}
