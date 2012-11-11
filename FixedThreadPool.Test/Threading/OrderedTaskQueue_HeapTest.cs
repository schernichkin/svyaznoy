using Svyaznoy.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Svyaznoy.Threading
{
    /// <summary>
    ///This is a test class for OrderedTaskQueue_HeapTest and is intended
    ///to contain all OrderedTaskQueue_HeapTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OrderedTaskQueue_HeapTest
    {
        [TestMethod]
        public void Test()
        {
            var heap = new OrderedTaskQueue_Heap_Accessor();
            heap.Enqueue(new TaskMock(), Priority.High);
        }
        // OrderedTaskQueue_Heap_Accessor
    }
}
