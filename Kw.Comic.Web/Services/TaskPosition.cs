using System.Diagnostics;

namespace KwC.Services
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public readonly struct TaskPosition
    {
        public readonly int TaskCount;
        public readonly int CurrentCount;

        public TaskPosition(int taskCount, int currentCount)
        {
            TaskCount = taskCount;
            CurrentCount = currentCount;
        }
        public override string ToString()
        {
            return $"{{{CurrentCount}, {TaskCount}}}";
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
