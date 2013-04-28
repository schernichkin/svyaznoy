using Svyaznoy.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Svyaznoy.Threading
{
    [TestClass]
    public class InterleavedTaskQueueTest : CommonTaskQueueTest<InterleavedTaskQueue>
    {
        [TestMethod]
        public void ConstructorTest()
        {
            CommonConstructorTest();
        }

        [TestMethod]
        public void TryDequeueTest_NoTask()
        {
            CommonDequeueNoTasksTest();
        }

        [TestMethod]
        public void EnqueueDequeueTest_LowMediumHigh()
        {
            CommonEnqueueDequeueTest_LowMediumHigh();
        }

        [TestMethod]
        public void EnqueueDequeueTest_HighMediumLow()
        {
            var highPriorityTasks = Enumerable.Range(0, 10).Select(i => (ITask)new TaskMock()).ToList();
            var mediumPriorityTasks = Enumerable.Range(0, 5).Select(i => (ITask)new TaskMock()).ToList();
            var lowPriorityTasks = Enumerable.Range(0, 5).Select(i => (ITask)new TaskMock()).ToList();

            RepeatTest(queue =>
            {
                EnqueueDequeueTest(queue,
                    highPriorityTasks.Select(task => new TaskWithPriority(task, Priority.High))
                        .Concat(mediumPriorityTasks.Select(task => new TaskWithPriority(task, Priority.Medium)))
                        .Concat(lowPriorityTasks.Select(task => new TaskWithPriority(task, Priority.Low))),
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

        #region Internal structure test

        [TestMethod]
        public void CountTest()
        {
            RepeatTest(queue =>
            {
                queue.Enqueue(new TaskMock(), Priority.High);
                queue.Enqueue(new TaskMock(), Priority.Medium);
                queue.Enqueue(new TaskMock(), Priority.Low);

                Assert.AreEqual(3, queue.Count);
                Assert.AreEqual(queue.Count, );

            });
        }

        #endregion

        protected override InterleavedTaskQueue CreateTaskQueue()
        {
            return new InterleavedTaskQueue();
        }
    }
}