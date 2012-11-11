using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Svyaznoy.Threading
{
    /// <summary>
    /// Min-heap
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Heap<T>
    {
        public Heap()
            : this(Comparer<T>.Default)
        {
        }

        public Heap(IComparer<T> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");

            m_Comparer = comparer;
        }

        public void Insert(T item)
        {
            if (Count == Capacity)
            {
                Grow();
            }

            Items[Count] = item;
            var index = Count;
            Count++;

            int parentIndex;
            while (index > 0 && Compare(index, parentIndex = ParentIndex(index)) < 0)
            {
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        public T Delete()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Heap is empty.");
            }

            var item = Items[0];

            if (--Count != 0)
            {
                Items[0] = Items[Count];

                var index = 0;
                var largest = 0;

                while (true)
                {
                    var child1Index = Child1Index(index);
                    var child2Index = Child2Index(index);

                    if (child1Index < Count && Compare(child1Index, largest) < 0)
                    {
                        largest = child1Index;
                    }

                    if (child2Index < Count && Compare(child2Index, largest) < 0)
                    {
                        largest = child2Index;
                    }

                    if (largest == index)
                    {
                        break;
                    }

                    Swap(index, largest);
                    index = largest;
                }
            }

            return item;
        }

        #region private int Count

        private int m_Count;

        public int Count { get { return m_Count; } private set { m_Count = value; } }

        #endregion

        private int Child1Index(int index)
        {
            return (index << 1) + 1;
        }

        private int Child2Index(int index)
        {
            return (index << 1) + 2;
        }

        private int Compare(int index1, int index2)
        {
            return Comparer.Compare(Items[index1], Items[index2]);
        }

        private void Grow()
        {
            var newItems = new T[Capacity == 0 ? 1 : Capacity << 1];
            Array.Copy(Items, newItems, Capacity);
            Items = newItems;
        }

        private static int ParentIndex(int index)
        {
            return (index - 1) >> 1;
        }

        private void Swap(int index1, int index2)
        {
            var item = Items[index1];
            Items[index1] = Items[index2];
            Items[index2] = item;
        }

        #region private int Capacity

        private int Capacity { get { return Items.Length; } }

        #endregion

        #region private T[] Items

        private T[] m_Items = new T[0];

        private T[] Items { get { return m_Items; } set { m_Items = value; } }

        #endregion

        #region private IComparer<T> Comparer

        private readonly IComparer<T> m_Comparer;

        private IComparer<T> Comparer { get { return m_Comparer; } }

        #endregion
    }
}