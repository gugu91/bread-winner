using System.Threading;

namespace SamplesShared.DummyExample
{
    public class WorkAvailableRepo
    {
        private int _count;
        public int ConsecutiveAvailableBatches { get; }

        public WorkAvailableRepo(int consecutiveAvailableBatches)
        {
            ConsecutiveAvailableBatches = consecutiveAvailableBatches;
        }

        public void Reset()
        {
            FormattedConsole.WriteLine($"Job Scheduler {Thread.CurrentThread.ManagedThreadId} -Work Arrived!!");
            Interlocked.Exchange(ref _count, -1);
        }

        public bool WorkAvailable()
        {
            Interlocked.Increment(ref _count);

            return _count < ConsecutiveAvailableBatches;
        }
    }
}