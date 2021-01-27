using System.Threading;
using BreadWinner.Threading;

namespace BreadWinner
{
    internal class StartupWorkBatch : IWorkBatch
    {
        private readonly IWorkBatch _workBatch;
        private readonly IStartupBarrier _barrier;

        internal StartupWorkBatch(IWorkBatch workBatch, IStartupBarrier barrier)
        {
            _workBatch = workBatch;
            _barrier = barrier;
        }

        public string Id => _workBatch.Id;

        public void DoFinally(WorkItemResult result, CancellationToken cancellationToken)
        {
            _workBatch.DoFinally(result, cancellationToken);
            _barrier?.Wait(cancellationToken);
        }
    }
}
