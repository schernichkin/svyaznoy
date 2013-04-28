using System;
using System.Threading;
using Svyaznoy.Threading;

namespace SampleApplication
{
    internal sealed class ActionAdapter : ITask
    {
        public ActionAdapter(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            m_Action = action;
        }

        #region ITask Members

        public void Execute()
        {
            Action();
        }

        #endregion

        #region private Action Action

        private readonly Action m_Action;

        private Action Action { get { return m_Action; } }

        #endregion
    }

    internal static class FixedThreadPoolExtensions
    {
        public static bool Execute(this FixedThreadPool fixedThreadPool, Action action, Priority priority)
        {
            if (fixedThreadPool == null) throw new ArgumentNullException("fixedThreadPool");

            return fixedThreadPool.Execute(new ActionAdapter(action), priority);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Main";
            var threadPool = new FixedThreadPool(10);

            Log("Sheduling low priority tasks..");
            ScheduleTasks(threadPool, 10, Priority.Low);

            Log("Sheduling medium priority tasks..");
            ScheduleTasks(threadPool, 50, Priority.Medium);

            Log("Sheduling high priority tasks..");
            ScheduleTasks(threadPool, 100, Priority.High);

            Log("Waiting for tasks to complete..");
            threadPool.Stop();
            Log("Done.");
        }

        private static void ScheduleTasks(FixedThreadPool threadPool, int taskCount, Priority priority)
        {
            if (threadPool == null) throw new ArgumentNullException("threadPool");

            var random = new Random();

            for (var i = 0; i < taskCount; i++)
            {
                var taskNum = i;
                var taskTime = random.Next(1000);
                    threadPool.Execute(() =>
                    {
                        Log("Executing Task {0} at {1} priority", taskNum, priority);
                        Thread.Sleep(taskTime);
                    },
                    priority);
            }
        }

        static void Log(string message, params object[] args)
        {
            message = string.Format(message, args);
            Console.WriteLine(String.Format("[{0}] {1}", Thread.CurrentThread.Name, message));
        }
    }
}
