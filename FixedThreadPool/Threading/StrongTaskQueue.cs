using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Svyaznoy.Threading
{
    /// <summary>
    /// Priority task queue which maintains strong task order.
    /// </summary>
    /// <remarks>
    /// This queue will return 1 medium priority task per 3 high-priority tasks but 
    /// will take in account enqueuing order.
    /// 
    /// E.g.: User enqueued 4 high-priority tasks then 2 med. priority then 4 more high-priority.
    /// These tasks will be dequeued in following order:
    /// - 4 high-priority tasks because if was enqueued before med. priority.
    /// - From this point queue will start to interleave tasks it will allow 3 more high-priority to go
    ///   before med. priority.
    /// - Fist med. priority task.
    /// - Remaining high-priority task.
    /// - Last med. priority task.
    /// </remarks>
    public sealed class StrongTaskQueue : ITaskQueue
    {
        private const int INTERLEAVE = 3;

        #region ITaskQueue Members

        public void Enqueue(ITask task, Priority priority)
        {
            if (task == null) throw new ArgumentNullException("task");

            switch (priority)
            {
                case Priority.High:
                    if (InterleaveCounter >= INTERLEAVE)
                    { // Interleave threshold hit (hence having at least one mid.priority task in the queue)
                        LastHighPriorityNode = TaskList.AddAfter(LastHighPriorityNode.Next, task);
                        InterleaveCounter = 1;
                    }
                    else
                    { // Interleave threshold not hit
                        if (LastHighPriorityNode != null)
                        {
                            LastHighPriorityNode = TaskList.AddAfter(LastHighPriorityNode, task);
                        }
                        else
                        {
                            LastHighPriorityNode = TaskList.AddFirst(task);
                        }

                        if (LastHighPriorityNode.Next != null && LastHighPriorityNode.Next != FirstLowPriorityNode)
                        { // has med.priority nodes
                            InterleaveCounter++;
                        }
                    }
                    break;
                case Priority.Medium:
                    // Adding mid.priority task before the fist low-priority, or to end of list, if no low-priority tasks
                    if (FirstLowPriorityNode != null)
                    {
                        TaskList.AddBefore(FirstLowPriorityNode, task);
                    }
                    else
                    {
                        TaskList.AddLast(task);
                    }
                    break;
                case Priority.Low:
                    // Low-priority always goes to the end of the list.
                    var node = TaskList.AddLast(task);
                    if (FirstLowPriorityNode == null)
                    {
                        FirstLowPriorityNode = node;
                    }
                    break;
                default:
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Unsupported priority value: {0}.",
                            priority),
                        "priority");
            }
        }

        public ITask TryDequeue()
        {
            if (TaskList.Count != 0)
            {
                var node = TaskList.First;

                if (node == LastHighPriorityNode)
                {
                    LastHighPriorityNode = null;
                }
                else if (node == FirstLowPriorityNode)
                {
                    FirstLowPriorityNode = node.Next;
                }
                else if (node.Next == FirstLowPriorityNode)
                { // no more med.priority tasks, reset interleave counter.
                    InterleaveCounter = 0;
                }

                TaskList.Remove(node);

                return node.Value;
            }
            else
            {
                return null;
            }
        }

        public int Count
        {
            get { return TaskList.Count; }
        }

        #endregion

        #region private int InterleaveCounter

        private int _InterleaveCounter;

        private int InterleaveCounter
        {
            [DebuggerStepThroughAttribute]
            get
            {
                return _InterleaveCounter;
            }
            [DebuggerStepThroughAttribute]
            set
            {
                _InterleaveCounter = value;
            }
        }

        #endregion

        #region private LinkedListNode<ITask> FirstLowPriorityNode

        private LinkedListNode<ITask> _FirstLowPriorityNode;

        private LinkedListNode<ITask> FirstLowPriorityNode
        {
            [DebuggerStepThroughAttribute]
            get
            {
                return _FirstLowPriorityNode;
            }
            [DebuggerStepThroughAttribute]
            set
            {
                _FirstLowPriorityNode = value;
            }
        }

        #endregion

        #region private LinkedListNode<ITask> LastHighPriorityNode

        private LinkedListNode<ITask> _LastHighPriorityNode;

        private LinkedListNode<ITask> LastHighPriorityNode
        {
            [DebuggerStepThroughAttribute]
            get
            {
                return _LastHighPriorityNode;
            }
            [DebuggerStepThroughAttribute]
            set
            {
                _LastHighPriorityNode = value;
            }
        }

        #endregion

        #region private LinkedList<ITask> TaskList

        private readonly LinkedList<ITask> _TaskList = new LinkedList<ITask>();

        private LinkedList<ITask> TaskList
        {
            [DebuggerStepThroughAttribute]
            get
            {
                return _TaskList;
            }
        }

        #endregion
    }
}