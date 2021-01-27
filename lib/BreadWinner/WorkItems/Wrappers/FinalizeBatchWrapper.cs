using System;
using System.Threading;

namespace BreadWinner.WorkItems.Wrappers
{
    internal class FinalizeBatchWrapper : IWorkBatch
    {
        private readonly Action<WorkItemResult, CancellationToken> _finalizeBatch;

        public string Id { get; }

        public FinalizeBatchWrapper(Action<WorkItemResult, CancellationToken> finalizeBatch, string batchId)
        {
            _finalizeBatch = finalizeBatch ?? throw new ArgumentNullException(nameof(finalizeBatch));
            Id = batchId ?? Guid.NewGuid().ToString();
        }

        public void DoFinally(WorkItemResult result, CancellationToken cancellationToken)
        {
            _finalizeBatch(result, cancellationToken);
        }
    }
}