namespace Svyaznoy.Threading
{
    internal enum ThreadPoolStatus : int
    {
        Running = 0,
        Stopping = 1,
        Stopped = 2,
    }
}