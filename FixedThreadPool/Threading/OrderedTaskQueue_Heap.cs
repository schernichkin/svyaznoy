using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Svyaznoy.Threading
{
    internal sealed class OrderedTaskQueue_Heap : ITaskQueue
    {
        private const int INTERLEAVE = 3;

        #region ITaskQueue Members

        public void Enqueue(ITask task, Priority priority)
        {
            switch (priority)
            {
                case Priority.High:
                    if (HasFutureMediumPriorityTasks)
                    {
                        if (InterleaveCounter == 0)
                        { // skipping medium-priority task slot
                            HighPriority++;
                            InterleaveCounter = (INTERLEAVE - 1);
                        }
                        else
                        {
                            InterleaveCounter--;
                        }
                    }
                    TaskHeap.Insert(new TaskEntry(task, HighPriority++));
                    break;
                case Priority.Medium:
                    if (HasFutureMediumPriorityTasks)
                    {
                        MediumPriority = MediumPriority + INTERLEAVE;
                    }
                    else
                    { 
                        MediumPriority = HighPriority + INTERLEAVE;
                    }
                    TaskHeap.Insert(new TaskEntry(task, LowPriority++));
                    break;
                case Priority.Low:
                    TaskHeap.Insert(new TaskEntry(task, LowPriority++));
                    break;
                default:
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Unsupported priority value: {0}.",
                            priority),
                        "priority");
            }
        }

        public ITask TryDequeue()
        {
            if (Count > 0)
            {
                return TaskHeap.Delete().Task;
            }
            else
            {
                //queue is empty, may reset priority counters
                HighPriority = long.MinValue;
                MediumPriority = long.MinValue;
                LowPriority = 0;
                return null;
            }
        }

        public int Count
        {
            get { return TaskHeap.Count; }
        }

        #endregion

        #region private bool HasFutureMediumPriorityTasks

        private bool HasFutureMediumPriorityTasks
        {
            get
            {
                return MediumPriority > HighPriority;
            }
        }

        #endregion

        #region private long HighPriority

        private long m_HighPriority = long.MinValue;

        public long HighPriority { get { return m_HighPriority; } set { m_HighPriority = value; } }

        #endregion

        #region private int InterleaveCounter

        private int m_InterleaveCounter;

        private int InterleaveCounter { get { return m_InterleaveCounter; } set { m_InterleaveCounter = value; } }

        #endregion

        #region private long MediumPriority

        private long m_MediumPriority = long.MinValue;

        private long MediumPriority { get { return m_MediumPriority; } set { m_MediumPriority = value; } }

        #endregion

        #region private long LowPriority

        private long m_LowPriority = 0;

        private long LowPriority { get { return m_LowPriority; } set { m_LowPriority = value; } }

        #endregion

        #region private Heap<TaskEntry> TaskHeap

        private readonly Heap<TaskEntry> m_TaskHeap = new Heap<TaskEntry>();

        private Heap<TaskEntry> TaskHeap { get { return m_TaskHeap; } }

        #endregion

        #region TaskEntry

        private struct TaskEntry : IComparable<TaskEntry>
        {
            public TaskEntry(ITask task, long priority)
            {
                if (task == null) throw new ArgumentNullException("task");

                m_Priority = priority;
                m_Task = task;
            }

            #region public ITask Task

            private readonly ITask m_Task;

            public ITask Task { get { return m_Task; } }

            #endregion

            #region public long Priority

            private readonly long m_Priority;

            public long Priority { get { return m_Priority; } }

            #endregion

            #region IComparable<TaskEntry> Members

            public int CompareTo(TaskEntry other)
            {
                return Comparer<long>.Default.Compare(this.Priority, other.Priority);
            }

            #endregion
        }

        #endregion
    }
}