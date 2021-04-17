using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Visiting
{
    /// <summary>
    /// 数据游标
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public interface IDataCursor<T> : INotifyPropertyChanged, IDisposable
#if NET461_OR_GREATER||NETSTANDARD2_0
        ,IAsyncEnumerable<T>
#endif
    {
        int Count { get; }

        int CurrentIndex { get; }

        T Current { get; }

        Task<bool> MoveAsync(int index);

        event Action<IDataCursor<T>, int> Moved;
    }
}
