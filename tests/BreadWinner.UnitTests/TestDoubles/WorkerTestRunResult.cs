using System.Threading;

namespace BreadWinner.UnitTests.TestDoubles
{
    public class WorkerTestRunResult
    {
        public ThreadState ThreadState { get; }
        public CancellationToken CancellationToken { get; }

        public WorkerTestRunResult(ThreadState threadState, CancellationToken cancellationToken)
        {
            ThreadState = threadState;
            CancellationToken = cancellationToken;
        }
    }
}