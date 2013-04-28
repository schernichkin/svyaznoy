using System;

namespace Svyaznoy.Threading
{
    public sealed class TaskWithPriority
    {
        public TaskWithPriority(ITask task, Priority priority)
        {
            if (task == null) throw new ArgumentNullException("task");

            m_Priority = priority;
            m_Task = task;
        }

        #region public Priority Priority

        private readonly Priority m_Priority;

        public Priority Priority { get { return m_Priority; } }

        #endregion

        #region public ITask Task

        private readonly ITask m_Task;

        public ITask Task { get { return m_Task; } }

        #endregion
    }
}
