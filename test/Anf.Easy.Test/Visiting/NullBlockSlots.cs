using Anf.Easy.Visiting;
using System;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    internal class NullBlockSlots : BlockSlots<object>
    {
        public NullBlockSlots(int size)
            : base(size)
        {
            Datas = new Func<object>[size];
        }
        public Func<object>[] Datas { get; }

        protected override Task<object> OnLoadAsync(int index)
        {
            return Task.FromResult(Datas[index]());
        }
    }
}
