using Svyaznoy.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Svyaznoy.Threading
{
    [TestClass]
    public class OrderedTaskHeapTest : CommonTaskQueueTest<OrderedTaskHeap>
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

        protected override OrderedTaskHeap CreateTaskQueue()
        {
            return new OrderedTaskHeap();
        }
    }
}