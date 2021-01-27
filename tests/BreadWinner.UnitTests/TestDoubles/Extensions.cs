using System;
using System.Threading;

namespace BreadWinner.UnitTests.TestDoubles
{
    public static class Extensions
    {
        public static WorkerTestRunResult TestRun(this IWorker workerUnderTest, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }

            var thread = new Thread(() => workerUnderTest.Start(cancellationTokenSource.Token));
            thread.Start();

            cancellationTokenSource.CancelAfter(new TimeSpan(0, 0, 0, 1));
            thread.TryJoin();

            return new WorkerTestRunResult(thread.ThreadState, cancellationTokenSource.Token);
        }

        public static void TryJoin(this Thread thread)
        {
            if (!thread.Join(new TimeSpan(0, 0, 0, 10)))
            {
                thread.Abort();
            }
        }
    }
}