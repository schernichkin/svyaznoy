using System;
using System.Collections.Generic;
using System.Globalization;

namespace Svyaznoy.Threading
{
    /// <summary>
    /// Priority task queue which does not maintain strong task order.
    /// </summary>
    /// <remarks>
    /// This queue will return 1 medium priority task per 3 high-priority tasks and 
    /// will return low priority tasks only if no higher-priority tasks in queue 
    /// without taking into account enqueuing order.
    /// 
    /// E.g. if user enqueued 1 med.-priority task, than 3 high-priority task, 
    /// first 3 high-priority tasks will be dequeued, than med.-priority task.
    /// 
    /// On other hand, if user has enqueded 4 high-priority tasks, and than 1
    /// med.-priority task, this queue will return 3 high-priority tasks, than
    /// 1 med.-priority task than last high-priority task despite it was enqueued
    /// before med.-priority task.
    /// </remarks>
    public sealed class WeakTaskQueue : ITaskQueue
    {
        private const int INTERLEAVE = 3;

        #region ITaskQueue Members

        public void Enqueue(ITask task, Priority priority)
        {
            if (task == null) throw new ArgumentNullException("task");

            switch (priority)
            {
                case Priority.High:
                    HighPriorityQueue.Enqueue(task);
                    break;
                case Priority.Medium:
                    MediumPriorityQueue.Enqueue(task);
                    break;
                case Priority.Low:
                    LowPriorityQueue.Enqueue(task);
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
            if (HighPriorityQueue.Count > 0)
            {
                if (InterleaveCounter >= INTERLEAVE)
                {
                    InterleaveCounter = 0;
                    return MediumPriorityQueue.Dequeue();
                }
                else
                {
                    if (MediumPriorityQueue.Count > 0)
                    {
                        InterleaveCounter++;
                    }
                    return HighPriorityQueue.Dequeue();
                }
            }
            else if (MediumPriorityQueue.Count > 0)
            {
                InterleaveCounter = 0;
                return MediumPriorityQueue.Dequeue();
            }
            else if (LowPriorityQueue.Count > 0)
            {
                InterleaveCounter = 0;
                return LowPriorityQueue.Dequeue();
            }
            else
            {
                InterleaveCounter = 0;
                return null;
            }
        }

        public int Count
        {
            get
            {
                return HighPriorityQueue.Count + MediumPriorityQueue.Count + LowPriorityQueue.Count;
            }
        }

        #endregion

        #region private Queue<ITask> HighPriorityQueue

        private readonly Queue<ITask> m_HighPriorityQueue = new Queue<ITask>();

        private Queue<ITask> HighPriorityQueue { get { return m_HighPriorityQueue; } }

        #endregion

        #region private int InterleaveCounter

        private int m_InterleaveCounter;

        private int InterleaveCounter { get { return m_InterleaveCounter; } set { m_InterleaveCounter = value; } }

        #endregion

        #region private Queue<ITask> LowPriorityQueue

        private readonly Queue<ITask> m_LowPriorityQueue = new Queue<ITask>();

        private Queue<ITask> LowPriorityQueue { get { return m_LowPriorityQueue; } }

        #endregion

        #region private Queue<ITask> MediumPriorityQueue

        private readonly Queue<ITask> m_MediumPriorityQueue = new Queue<ITask>();

        private Queue<ITask> MediumPriorityQueue { get { return m_MediumPriorityQueue; } }

        #endregion
    }
}