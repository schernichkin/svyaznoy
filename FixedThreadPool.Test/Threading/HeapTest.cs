using Svyaznoy.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace Svyaznoy.Threading
{
    [TestClass]
    public class HeapTest
    {
        [TestMethod]
        public void DirectOrder()
        {
            var heap = new Heap_Accessor<int>();

            for (var i = 99; i >= 0; i--)
            {
                heap.Insert(i);
            }

            Assert.AreEqual(100, heap.Count);

            for (var i = 0; i < 100; i++)
            {
                Assert.AreEqual(i, heap.Delete());
            }

            Assert.AreEqual(0, heap.Count);
        }

        [TestMethod]
        public void ReversedOrder()
        {
            var heap = new Heap_Accessor<int>();

            for (var i = 0; i < 100; i++)
            {
                heap.Insert(i);
            }

            Assert.AreEqual(100, heap.Count);

            for (var i = 0; i < 100; i++)
            {
                Assert.AreEqual(i, heap.Delete());
            }

            Assert.AreEqual(0, heap.Count);
        }

        [TestMethod]
        public void RandomOrder()
        {
            var heap = new Heap_Accessor<int>();

            var array = new int[100];
            for (var i = 0; i < 100; i++)
            {
                array[i] = i;
            }
            Permute(array, new Random(100500));

            for (var i = 0; i < 100; i++)
            {
                heap.Insert(array[i]);
            }
            
            Assert.AreEqual(100, heap.Count);

            for (var i = 0; i < 100; i++)
            {
                Assert.AreEqual(i, heap.Delete());
            }

            Assert.AreEqual(0, heap.Count);
        }

        private static void Permute<T>(T[] array, Random generator)
        {
            for (var i = array.Length - 1; i > 0; i--)
            {
                Swap(array, i, generator.Next(i + 1));
            }
        }

        private static void Swap<T>(T[] array, int index1, int index2)
        {
            var item = array[index1];
            array[index1] = array[index2];
            array[index2] = item ;
        }
    }
}