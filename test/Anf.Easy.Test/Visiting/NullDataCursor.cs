using Anf.Easy.Visiting;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    internal class NullDataCursor : DataCursorBase<object>
    {
        public NullDataCursor(object[] datas)
        {
            Datas = datas;
        }
        public bool IsOnMoved { get; set; }

        public bool IsOnSkiped { get; set; }

        public object[] Datas { get; }

        public override int Count => Datas.Length;

        protected override Task<object> LoadAsync(int index)
        {
            return Task.FromResult(Datas[index]);
        }
        protected override void OnMoved(int index, object value)
        {
            IsOnMoved = true;
        }
        protected override void OnSkipSet(int index, object value)
        {
            IsOnSkiped = true;
        }
    }
}
