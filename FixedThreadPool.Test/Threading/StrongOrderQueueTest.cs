using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Svyaznoy.Threading
{
    [TestClass]
    public class StrongOrderQueueTest: CommonTaskQueueTest<StrongTaskQueue>
    {
        [TestMethod]
        public void ConstructorTest()
        {
            CommonConstructorTest();
        }

        [TestMethod]
        public void CountTest()
        {
            CommonCountTest();
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
                            highPriorityTasks[3],
                            highPriorityTasks[4],
                            highPriorityTasks[5],
                            highPriorityTasks[6],
                            highPriorityTasks[7],
                            highPriorityTasks[8],
                            highPriorityTasks[9],
                            mediumPriorityTasks[0],
                            mediumPriorityTasks[1],
                            mediumPriorityTasks[2],
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

        [TestMethod]
        public void EnqueueDequeueTest_Mixed()
        {
            var highPriorityTasks = Enumerable.Range(0, 10).Select(i => new TaskWithPriority(new TaskMock(), Priority.High)).ToList();
            var mediumPriorityTasks = Enumerable.Range(0, 5).Select(i => new TaskWithPriority(new TaskMock(), Priority.Medium)).ToList();
            var lowPriorityTasks = Enumerable.Range(0, 5).Select(i => new TaskWithPriority(new TaskMock(), Priority.Low)).ToList();

            RepeatTest(queue =>
            {
                EnqueueDequeueTest(queue,
                    new[] { lowPriorityTasks[0],
                            highPriorityTasks[0],
                            highPriorityTasks[1], 
                            highPriorityTasks[2],
                            highPriorityTasks[3],
                            highPriorityTasks[4],
                            lowPriorityTasks[1],
                            mediumPriorityTasks[0],
                            mediumPriorityTasks[1],
                            mediumPriorityTasks[2],
                            lowPriorityTasks[2],
                            highPriorityTasks[5],
                            highPriorityTasks[6],
                            highPriorityTasks[7],
                            mediumPriorityTasks[3],
                            mediumPriorityTasks[4],
                            highPriorityTasks[8],
                            highPriorityTasks[9],
                            lowPriorityTasks[3],
                            lowPriorityTasks[4] },
                new[] { highPriorityTasks[0].Task,
                            highPriorityTasks[1].Task, 
                            highPriorityTasks[2].Task,
                            highPriorityTasks[3].Task,
                            highPriorityTasks[4].Task,
                            highPriorityTasks[5].Task,
                            highPriorityTasks[6].Task,
                            highPriorityTasks[7].Task,
                            mediumPriorityTasks[0].Task,
                            highPriorityTasks[8].Task,
                            highPriorityTasks[9].Task,
                            mediumPriorityTasks[1].Task,
                            mediumPriorityTasks[2].Task,
                            mediumPriorityTasks[3].Task,
                            mediumPriorityTasks[4].Task,
                            lowPriorityTasks[0].Task,
                            lowPriorityTasks[1].Task,
                            lowPriorityTasks[2].Task,
                            lowPriorityTasks[3].Task,
                            lowPriorityTasks[4].Task });

                AssertIsEmpty(queue);
            });
        }

        protected override StrongTaskQueue CreateTaskQueue()
        {
            return new StrongTaskQueue();
        }
    }
}