using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    /// <summary>
    /// 数据游标
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public interface IDataCursor<T> : INotifyPropertyChanged, IDisposable
    {
        int Count { get; }

        int CurrentIndex { get; }

        T Current { get; }

        Task<bool> MoveAsync(int index);

        event Action<DataCursorBase<T>, int> Moved;
    }
}
