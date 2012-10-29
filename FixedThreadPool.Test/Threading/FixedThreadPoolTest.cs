using Svyaznoy.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System;

namespace Svyaznoy.Threading
{
    [TestClass()]
    public class FixedThreadPoolTest
    {
        [TestMethod()]
        public void ConstructorTest_Force()
        {
            var target = CreateFixedThreadPool(10, true);

            Wait();
            
            Assert.IsNotNull(target.TaskQueue);
            Assert.AreEqual(ThreadPoolStatus_Accessor.Running, target.Status);
            Assert.AreEqual(10, target.MaxThreadCount);
            Assert.AreEqual(0, target.DeferedThreadCount);
            Assert.AreEqual(10, target.IdleThreadCount);
        }

        [TestMethod()]
        public void ConstructorTest_NoForce()
        {
            var target = CreateFixedThreadPool(10, false);

            Wait();

            Assert.IsNotNull(target.TaskQueue);
            Assert.AreEqual(ThreadPoolStatus_Accessor.Running, target.Status);
            Assert.AreEqual(10, target.MaxThreadCount);
            Assert.AreEqual(10, target.DeferedThreadCount);
            Assert.AreEqual(0, target.IdleThreadCount);
        }

        [TestMethod()]
        public void ReuseOneThreadTest()
        {
            var target = CreateFixedThreadPool(4, false);
            
            Assert.AreEqual(true, target.Execute(new TaskMock(), Priority.High));
            Wait();
            Assert.AreEqual(3, target.DeferedThreadCount);

            Assert.AreEqual(true, target.Execute(new TaskMock(), Priority.High));
            Wait();
            Assert.AreEqual(3, target.DeferedThreadCount);
            
            Assert.AreEqual(true, target.Execute(new TaskMock(), Priority.High));
            Wait();
            Assert.AreEqual(3, target.DeferedThreadCount);

            target.Stop();
            AssertIsProperlyStopped(target);
        }

        [TestMethod()]
        public void CreateThreadsTest()
        {
            var target = CreateFixedThreadPool(4, false);
            var heavyTask = new TaskMock(Wait);

            Assert.AreEqual(true, target.Execute(heavyTask, Priority.High));
            Assert.AreEqual(true, target.Execute(heavyTask, Priority.High));
            Assert.AreEqual(true, target.Execute(heavyTask, Priority.High));
            Wait();
            Assert.AreEqual(1, target.DeferedThreadCount);

            target.Stop();
            AssertIsProperlyStopped(target);
        }

        [TestMethod()]
        public void StopTest_NoTasks()
        {
            var target = CreateFixedThreadPool(4, false);
            target.Stop();
            AssertIsProperlyStopped( target);
        }

        [TestMethod()]
        public void StopTest_TasksCompleted()
        {
            var target = CreateFixedThreadPool(4, false);
            Assert.AreEqual(true, target.Execute(new TaskMock(), Priority.High));
            Assert.AreEqual(true, target.Execute(new TaskMock(), Priority.High));
            Assert.AreEqual(true, target.Execute(new TaskMock(), Priority.High));
            Wait();
            target.Stop();
            AssertIsProperlyStopped(target);
        }

        [TestMethod()]
        public void StopTest_MultiplyWaiters()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ScheduleAfterStopTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void BasicSequenceTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ReuseThreadsTest()
        {
            var target = CreateFixedThreadPool(4, false);
            var heavyTask = new TaskMock(Wait);

            Assert.AreEqual(true, target.Execute(heavyTask, Priority.High));
            Assert.AreEqual(true, target.Execute(heavyTask, Priority.High));
            Assert.AreEqual(true, target.Execute(heavyTask, Priority.High));
            Wait();
            Assert.AreEqual(1, target.DeferedThreadCount);
            Wait(); // Allowing threads to finish.

            // Now will attemp to reuse it
            Assert.AreEqual(true, target.Execute(heavyTask, Priority.High));
            Assert.AreEqual(true, target.Execute(heavyTask, Priority.High));
            Assert.AreEqual(true, target.Execute(heavyTask, Priority.High));
            Wait();
            Assert.AreEqual(1, target.DeferedThreadCount);

            target.Stop();
            AssertIsProperlyStopped(target);
        }

        private void AssertIsProperlyStopped(FixedThreadPool_Accessor target)
        {
            Assert.AreEqual(ThreadPoolStatus_Accessor.Stopped, target.Status);
            Assert.AreEqual(0, target.TaskQueue.Count);
            Assert.AreEqual(0, target.IdleThreadCount);
            Assert.AreEqual(target.MaxThreadCount, target.DeferedThreadCount);
        }

        private void Wait()
        {
            Thread.Sleep(500);
        }

        private FixedThreadPool_Accessor CreateFixedThreadPool(int treadCount, bool forceThreadCreation)
        {
            var taskQueue = new ITaskQueue_Impl(new PrivateObject(new TaskQueue_3_1_0()));
            return new FixedThreadPool_Accessor(null, treadCount, taskQueue, forceThreadCreation);
        }
    }
}
