using Svyaznoy.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Svyaznoy.Threading
{
    [TestClass()]
    public class InterleavedTaskQueueTest
    {
        [TestMethod()]
        public void ConstructorTest()
        {
            var target = new InterleavedTaskQueue_Accessor();
            Assert.AreEqual(0, target.InterleaveCounter);
            AssertIsEmpty(target);
        }

        [TestMethod()]
        public void TryDequeueTest_NoTask()
        {
            var target = new InterleavedTaskQueue_Accessor();
            Assert.AreEqual(null, target.TryDequeue());
        }

        [TestMethod()]
        public void EnqueueTest_High()
        {
            var target = new InterleavedTaskQueue_Accessor();
            var task = new TaskMock();
            target.Enqueue(task, Priority.High);
            Assert.AreEqual(task, target.HighPriorityQueue.Peek());
            Assert.AreEqual(0, target.MediumPriorityQueue.Count);
            Assert.AreEqual(0, target.LowPriorityQueue.Count);
        }

        [TestMethod()]
        public void EnqueueTest_Medium()
        {
            var target = new InterleavedTaskQueue_Accessor();
            var task = new TaskMock();
            target.Enqueue(task, Priority.Medium);
            Assert.AreEqual(task, target.MediumPriorityQueue.Peek());
            Assert.AreEqual(0, target.HighPriorityQueue.Count);
            Assert.AreEqual(0, target.LowPriorityQueue.Count);
        }

        [TestMethod()]
        public void EnqueueTest_Low()
        {
            var target = new InterleavedTaskQueue_Accessor();
            var task = new TaskMock();
            target.Enqueue(task, Priority.Low);
            Assert.AreEqual(task, target.LowPriorityQueue.Peek());
            Assert.AreEqual(0, target.HighPriorityQueue.Count);
            Assert.AreEqual(0, target.MediumPriorityQueue.Count);
        }

        [TestMethod()]
        public void DequeueTest_Interleave()
        {
            var target = new InterleavedTaskQueue_Accessor();
            var tasksH = Enumerable.Range(0, 6).Select(i => (ITask)new TaskMock()).ToList();
            var tasksL = Enumerable.Range(0, 2).Select(i => (ITask)new TaskMock()).ToList();

            target.HighPriorityQueue.EnqueueAll(tasksH);
            target.MediumPriorityQueue.EnqueueAll(tasksL);

            Assert.AreEqual(tasksH[0], target.TryDequeue());
            Assert.AreEqual(1, target.InterleaveCounter);
            Assert.AreEqual(tasksH[1], target.TryDequeue());
            Assert.AreEqual(2, target.InterleaveCounter);
            Assert.AreEqual(tasksH[2], target.TryDequeue());
            Assert.AreEqual(3, target.InterleaveCounter);

            Assert.AreEqual(tasksL[0], target.TryDequeue());
            Assert.AreEqual(0, target.InterleaveCounter);

            Assert.AreEqual(tasksH[3], target.TryDequeue());
            Assert.AreEqual(1, target.InterleaveCounter);
            Assert.AreEqual(tasksH[4], target.TryDequeue());
            Assert.AreEqual(2, target.InterleaveCounter);
            Assert.AreEqual(tasksH[5], target.TryDequeue());
            Assert.AreEqual(3, target.InterleaveCounter);

            Assert.AreEqual(tasksL[1], target.TryDequeue());
            Assert.AreEqual(0, target.InterleaveCounter);

            AssertIsEmpty(target);
        }

        [TestMethod()]
        public void DequeueTest_NoInterleave()
        {
            var target = new InterleavedTaskQueue_Accessor();
            var tasksH = Enumerable.Range(0, 6).Select(i => (ITask)new TaskMock()).ToList();
            
            target.HighPriorityQueue.EnqueueAll(tasksH);

            Assert.AreEqual(tasksH[0], target.TryDequeue());
            Assert.AreEqual(0, target.InterleaveCounter);
            Assert.AreEqual(tasksH[1], target.TryDequeue());
            Assert.AreEqual(0, target.InterleaveCounter);
            Assert.AreEqual(tasksH[2], target.TryDequeue());
            Assert.AreEqual(0, target.InterleaveCounter);
            Assert.AreEqual(tasksH[3], target.TryDequeue());
            Assert.AreEqual(0, target.InterleaveCounter);
            Assert.AreEqual(tasksH[4], target.TryDequeue());
            Assert.AreEqual(0, target.InterleaveCounter);
            Assert.AreEqual(tasksH[5], target.TryDequeue());
            Assert.AreEqual(0, target.InterleaveCounter);

            AssertIsEmpty(target);
        }

        [TestMethod()]
        public void TryDequeueTest_ResetInterleaveCounterOnNonPriorityTasks()
        {
            var tasksH = Enumerable.Range(0, 3).Select(i => (ITask)new TaskMock()).ToList();
            var tasksM = new TaskMock();
            var tasksL = new TaskMock();


        }

        [TestMethod()]
        public void DequeueTest_BasicPriority()
        {
            var target = new InterleavedTaskQueue_Accessor();
            var taskH = new TaskMock();
            var taskM = new TaskMock();
            var taskL = new TaskMock();

            target.Enqueue(taskH, Priority.High);
            target.Enqueue(taskM, Priority.Medium);
            target.Enqueue(taskL, Priority.Low);

            Assert.AreEqual(taskH, target.TryDequeue());
            Assert.AreEqual(taskM, target.TryDequeue());
            Assert.AreEqual(taskL, target.TryDequeue());

            AssertIsEmpty(target);
        }

        private void AssertIsEmpty(InterleavedTaskQueue_Accessor taskQueue)
        {
            Assert.AreEqual(0, taskQueue.Count);
            Assert.AreEqual(0, taskQueue.HighPriorityQueue.Count);
            Assert.AreEqual(0, taskQueue.MediumPriorityQueue.Count);
            Assert.AreEqual(0, taskQueue.LowPriorityQueue.Count);
        }
    }
}