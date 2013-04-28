using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Svyaznoy.Threading
{
    [TestClass]
    public class WeakTaskQueueTest : CommonTaskQueueTest<WeakTaskQueue>
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

        protected override WeakTaskQueue CreateTaskQueue()
        {
            return new WeakTaskQueue();
        }
    }
}