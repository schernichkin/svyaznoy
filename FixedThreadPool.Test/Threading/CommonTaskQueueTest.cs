using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Svyaznoy.Threading
{
    public abstract class CommonTaskQueueTest<TTaskQueue>
        where TTaskQueue : ITaskQueue
    {
        protected virtual void AssertIsEmpty(TTaskQueue target)
        {
            if (target == null) throw new ArgumentNullException("target");

            Assert.AreEqual(0, target.Count);
        }

        protected abstract TTaskQueue CreateTaskQueue();

        protected void EnqueueDequeueTest(TTaskQueue queue, IEnumerable<TaskWithPriority> tasks, IEnumerable<ITask> exceptedOrder)
        {
            if (queue == null) throw new ArgumentNullException("queue");
            if (tasks == null) throw new ArgumentNullException("tasks");
            if (exceptedOrder == null) throw new ArgumentNullException("exceptedOrder");

            foreach (var taskWithPriority in tasks)
            {
                queue.Enqueue(taskWithPriority.Task, taskWithPriority.Priority);
            }

            foreach (var exceptedTask in exceptedOrder)
            {
                var actualTask = queue.TryDequeue();
                Assert.AreEqual(exceptedTask, actualTask);
            }
        }

        /// <summary>
        /// Repeat test on the same queue several times to ensure 
        /// test does not corrupt queue internal state.
        /// </summary>
        /// <param name="test"></param>
        protected void RepeatTest(Action<TTaskQueue> test)
        {
            if (test == null) throw new ArgumentNullException("test");

            var taskQueue = CreateTaskQueue();
            for (var i = 0; i < 3; i++)
            {
                test(taskQueue);
            }
        }

        #region Common tests
        // These tests should be called from the derived class test methods.

        protected void CommonConstructorTest()
        {
            var taskQueue = CreateTaskQueue();
            AssertIsEmpty(taskQueue);
        }

        protected void CommonCountTest()
        {
            RepeatTest(queue =>
            {
                queue.Enqueue(new TaskMock(), Priority.High);
                queue.Enqueue(new TaskMock(), Priority.Medium);
                queue.Enqueue(new TaskMock(), Priority.Low);

                Assert.AreEqual(3, queue.Count);
                queue.TryDequeue();
                queue.TryDequeue();
                queue.TryDequeue();
            });
        }


        protected void CommonDequeueNoTasksTest()
        {
            RepeatTest(qeue => Assert.AreEqual(null, qeue.TryDequeue()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This test will first equeue low-priority tasks, than medium-priority
        /// than high-priority. This mean method should produce same result
        /// both for ordered-in-time and interleaved queues.
        /// </remarks>
        protected void CommonEnqueueDequeueTest_LowMediumHigh()
        {
            var highPriorityTasks = Enumerable.Range(0, 10).Select(i => (ITask)new TaskMock("H_" + i.ToString())).ToList();
            var mediumPriorityTasks = Enumerable.Range(0, 5).Select(i => (ITask)new TaskMock("M_" + i.ToString())).ToList();
            var lowPriorityTasks = Enumerable.Range(0, 5).Select(i => (ITask)new TaskMock("L_" + i.ToString())).ToList();

            RepeatTest(queue =>
                {
                    EnqueueDequeueTest(queue,
                        lowPriorityTasks.Select(task => new TaskWithPriority(task, Priority.Low))
                            .Concat(mediumPriorityTasks.Select(task => new TaskWithPriority(task, Priority.Medium)))
                            .Concat(highPriorityTasks.Select(task => new TaskWithPriority(task, Priority.High))),
                        new[] { highPriorityTasks[0],
                                highPriorityTasks[1], 
                                highPriorityTasks[2],
                                mediumPriorityTasks[0],
                                highPriorityTasks[3],
                                highPriorityTasks[4],
                                highPriorityTasks[5],
                                mediumPriorityTasks[1],
                                highPriorityTasks[6],
                                highPriorityTasks[7],
                                highPriorityTasks[8],
                                mediumPriorityTasks[2],
                                highPriorityTasks[9],
                                mediumPriorityTasks[3],
                                mediumPriorityTasks[4],
                                lowPriorityTasks[0],
                                lowPriorityTasks[1],
                                lowPriorityTasks[2],
                                lowPriorityTasks[3],
                                lowPriorityTasks[4] });
                    AssertIsEmpty(queue);
                });
        }

        #endregion
    }
}