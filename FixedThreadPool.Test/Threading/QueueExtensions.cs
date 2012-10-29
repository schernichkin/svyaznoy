using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Svyaznoy.Threading;

namespace Svyaznoy.Threading
{
    internal static class QueueExtensions
    {
        public static void EnqueueAll<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            if (queue == null) throw new ArgumentNullException("queue");
            if (items == null) throw new ArgumentNullException("items");

            foreach (var item in items)
            {
                queue.Enqueue(item);
            }
        }
    }
}