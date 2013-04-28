namespace Svyaznoy.Threading
{
    /// <summary>
    /// Task queue for the <see cref="FixedThreadPool">FixedThreadPool</see>
    /// </summary>
    /// <remarks>
    /// Task queue does not need to be thread safe because <see cref="FixedThreadPool">FixedThreadPool</see>
    /// will synchronize access.
    /// </remarks>
    public interface ITaskQueue
    {
        /// <summary>
        /// Enqueue task with the specified priority.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="priority"></param>
        void Enqueue(ITask task, Priority priority);
        
        /// <summary>
        /// Attempts to dequeue task.
        /// </summary>
        /// <returns>
        /// Task of null reference if not tasks in the queue
        /// </returns>
        ITask  TryDequeue();

        /// <summary>
        /// Number of tasks in queue.
        /// </summary>
        int Count { get; }
    }
}