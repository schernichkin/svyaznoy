using System;
using System.Threading;
using System.Globalization;

namespace Svyaznoy.Threading
{
    public class FixedThreadPool
    {
        public FixedThreadPool(int maxThreadCount)
            : this(NewDefaultName(), maxThreadCount, new InterleavedTaskQueue(), false)
        {
        }

        public bool Execute(ITask task, Priority priority)
        {
            lock (TaskQueue)
            {
                if (Status == ThreadPoolStatus.Running)
                {
                    // Considering thread pool has an unoccupied thread if the number of 
                    // thread's waiting for queue event is greater, than task count.
                    // This is not 100% accurate during the high load, but will prevent  
                    // unnecessary thread's creation on low load.
                    var hasFreeThreads = TaskQueue.Count < IdleThreadCount;

                    TaskQueue.Enqueue(task, priority);

                    if (hasFreeThreads)
                    {
                        // Unlocking one waiting thread to allow it peek a task.
                        Monitor.Pulse(TaskQueue);
                    }
                    else
                    {
                        ThreadNeeded();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Stop()
        {
            lock (TaskQueue)
            {
                if (DeferedThreadCount == MaxThreadCount)
                { // No worker threads has been started, task queue is guaranteed to be empty.
                    Status = ThreadPoolStatus.Stopped;
                    return;
                }
                else
                {
                    switch (Status)
                    {
                        case ThreadPoolStatus.Running:
                            Status = ThreadPoolStatus.Stopping;
                            // Unlocking worker threads to allow them exit.
                            Monitor.PulseAll(TaskQueue);
                            goto case ThreadPoolStatus.Stopping;
                        case ThreadPoolStatus.Stopping:
                            // Waiting completion signal from worker thread
                            Monitor.Wait(TaskQueue);
                            break;
                    }
                }
            }
        }

        internal FixedThreadPool(string name, int maxThreadCount, ITaskQueue taskQueue, bool forceThreadCreation)
        {
            if (maxThreadCount <= 0) throw new ArgumentOutOfRangeException("maxThreadCount", "Maximum thread count should be greater than zero.");
            if (taskQueue == null) throw new ArgumentNullException("taskQueue");

            m_DeferedThreadCount = maxThreadCount;
            m_MaxThreadCount = maxThreadCount;
            m_Name = name;
            m_TaskQueue = taskQueue;

            if (forceThreadCreation)
            {
                for (var i = 0; i < MaxThreadCount; i++)
                {
                    ThreadNeeded();
                }
            }
        }

        private void ThreadNeeded()
        {
            if (DeferedThreadCount > 0)
            {
                var thread = new Thread(ThreadStart);
                var threadNumber = MaxThreadCount - DeferedThreadCount;
                thread.Name = Name + " " + threadNumber.ToString(CultureInfo.InvariantCulture);
                thread.Start();
                DeferedThreadCount--;
            }
        }

        private void ThreadStart()
        {
            ITask task;

            while ((task = TryGetTask()) != null)
            {
                try
                {
                    task.Execute();
                }
                catch
                {
                    ThreadTerminate();
                    throw;
                }
            }

            ThreadStop();
        }

        private void ThreadStop()
        {
            lock (TaskQueue)
            {
                DeferedThreadCount++;
                if (DeferedThreadCount == MaxThreadCount && Status == ThreadPoolStatus.Stopping)
                {
                    Status = ThreadPoolStatus.Stopped;
                    // Unlocking all threads waiting in the Stop() method.
                    Monitor.PulseAll(TaskQueue);
                }
            }
        }

        private void ThreadTerminate()
        {
            lock (TaskQueue)
            {
                DeferedThreadCount++;

                if (TaskQueue.Count > IdleThreadCount)
                { // Starting new thread in place of thread being terminated if needed..
                    ThreadNeeded();
                }
                else if (TaskQueue.Count == 0 && DeferedThreadCount == MaxThreadCount && Status == ThreadPoolStatus.Stopping)
                { // or transfering to stopped status if it was last task in the queue and pool in stopping status
                    Status = ThreadPoolStatus.Stopped;
                    // Unlocking all threads waiting in the Stop() method.
                    Monitor.PulseAll(TaskQueue);
                }
            }
        }

        private ITask TryGetTask()
        {
            lock (TaskQueue)
            {
                while (true)
                {
                    ITask task = TaskQueue.TryDequeue();
                    if (task == null && Status == ThreadPoolStatus.Running)
                    {
                        IdleThreadCount++;
                        // Waiting for the task enqueue signal.
                        Monitor.Wait(TaskQueue);
                        IdleThreadCount--;
                    }
                    else
                    {
                        return task;
                    }
                }
            }
        }

        #region private int DeferedThreadCount

        private volatile int m_DeferedThreadCount;

        private int DeferedThreadCount { get { return m_DeferedThreadCount; } set { m_DeferedThreadCount = value; } }

        #endregion

        #region private int IdleThreadCount

        private volatile int m_IdleThreadCount;

        private int IdleThreadCount { get { return m_IdleThreadCount; } set { m_IdleThreadCount = value; } }

        #endregion

        #region private int MaxThreadCount

        private readonly int m_MaxThreadCount;

        private int MaxThreadCount { get { return m_MaxThreadCount; } }

        #endregion

        #region private string Name

        private readonly string m_Name;

        private string Name { get { return m_Name; } }

        #endregion

        #region private ThreadPoolStatus Status

        private volatile ThreadPoolStatus m_Status;

        private ThreadPoolStatus Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
            }
        }

        #endregion

        #region private ITaskQueue TaskQueue

        private readonly ITaskQueue m_TaskQueue;

        private ITaskQueue TaskQueue { get { return m_TaskQueue; } }

        #endregion

        #region name generator

        private static long InstanceCounter;

        private static string NewDefaultName()
        {
            var instanceNumber = Interlocked.Increment(ref InstanceCounter) - 1;
            return "FixedThreadPool " + instanceNumber.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}