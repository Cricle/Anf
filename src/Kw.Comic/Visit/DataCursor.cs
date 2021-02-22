using Kw.Core.Input;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public class DataCursor<T> : ViewModelBase,IDisposable
    {
        private int index = -1;

        public DataCursor(ImmutableArray<T> datas)
        {
            Datas = datas;
            Length = Datas.Length;
        }
        public DataCursor(IEnumerable<T> datas)
        {
            if (datas is null)
            {
                throw new ArgumentNullException(nameof(datas));
            }

            Datas = datas.ToImmutableArray();
            Length = Datas.Length;
        }

        private T current;

        public ImmutableArray<T> Datas { get; }

        public int Length { get; }

        public int Index
        {
            get => index;
            private set
            {
                RaisePropertyChanged(ref index, value);
            }
        }

        public bool IsFirst => index == 0;

        public bool IsLast => index == Datas.Length - 1;

        public T Current 
        {
            get => current;
            private set => RaisePropertyChanged(ref current, value);
        }

        public event Action<DataCursor<T>, int> IndexChanged;

        public T this[int idx]
        {
            get => Datas[idx];
        }

        public virtual void Dispose()
        {
        }

        public async Task<bool> SetIndexAsync(int idx)
        {
            if (idx < 0 || idx >= Datas.Length)
            {
                return false;
            }
            if (idx == index)
            {
                return true;
            }
            var i = index;
            if (Interlocked.CompareExchange(ref index, i, idx) == i)
            {
                await LoadIndexAsync(idx);
                Index = idx;
                Current = this[idx];
                RaisePropertyChanged(nameof(Index));
                IndexChanged?.Invoke(this, i);
                return true;
            }
            return false;
        }
        public virtual Task LoadIndexAsync(int index)
        {
            return Task.CompletedTask;
        }
    }
}
