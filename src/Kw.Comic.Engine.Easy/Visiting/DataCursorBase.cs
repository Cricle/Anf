using System;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public abstract class DataCursorBase<T> : ObserableObject, IDataCursor<T>
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

        public event Action<DataCursorBase<T>,int> Moved;
        public virtual void Dispose()
        {
        }

        public async Task<bool> MoveAsync(int index)
        {
            if (!this.IsInRange(index))
            {
                return false;
            }
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
        protected void OnSkipSet(int index,T value)
        {

        }
        protected void OnMoved(int index,T value)
        {

        }
        protected abstract Task<T> LoadAsync(int index);
    }
}
