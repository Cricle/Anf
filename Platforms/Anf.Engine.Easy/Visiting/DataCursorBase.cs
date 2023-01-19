using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Easy.Visiting
{
    public abstract class DataCursorBase<T> : ObserverObject, IDataCursor<T>
#if NET461_OR_GREATER || NETSTANDARD2_0
        , IAsyncEnumerable<T>
#endif
    {
        private int currentIndex = -1;
        private T current;

        private bool enableSafeSet;
        /// <summary>
        /// 是否启用原始值设值规则
        /// </summary>
        public bool EnableSafeSet
        {
            get { return enableSafeSet; }
            set => RaisePropertyChanged(ref enableSafeSet, value);
        }


        public abstract int Count { get; }

        public int CurrentIndex
        {
            get => currentIndex;
            private set => RaisePropertyChanged(ref currentIndex, value);
        }

        public T Current
        {
            get => current;
            private set => RaisePropertyChanged(ref current, value);
        }

        public event Action<IDataCursor<T>,int> Moved;

        public event Action<IDataCursor<T>, int> Moving;
        public event Action<IDataCursor<T>, int> MoveComplated;

        public virtual void Dispose()
        {
        }

        public async Task<bool> MoveAsync(int index)
        {
            if (!this.IsInRange(index))
            {
                OnSkipSet(index, default(T));
                return false;
            }
            Moving?.Invoke(this, index);
            try
            {

                var origin = CurrentIndex;
                var value = await LoadAsync(index);
                if (!enableSafeSet || origin == CurrentIndex)
                {
                    CurrentIndex = index;
                    Current = value;
                    OnMoved(index, value);
                    Moved?.Invoke(this, index);
                }
                else
                {
                    OnSkipSet(index, value);
                }
                return true;
            }
            finally
            {
                MoveComplated?.Invoke(this, index);
            }
        }
        protected virtual void OnSkipSet(int index,T value)
        {

        }
        protected virtual void OnMoved(int index,T value)
        {

        }
        protected abstract Task<T> LoadAsync(int index);

#if NET461_OR_GREATER||NETSTANDARD2_0
        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var index = 0;
            while (!cancellationToken.IsCancellationRequested && index < Count)
            {
                yield return await LoadAsync(index);
                index++;
            }
        }
#endif
    }
}
