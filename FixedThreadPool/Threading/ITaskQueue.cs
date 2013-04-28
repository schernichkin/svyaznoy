namespace Svyaznoy.Threading
{
    public interface ITaskQueue
    {
        void Enqueue(ITask task, Priority priority);
        
        ITask  TryDequeue();

        int Count { get; }
    }
}