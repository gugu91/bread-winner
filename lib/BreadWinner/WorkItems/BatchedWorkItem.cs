using System;
using System.Threading;
using BreadWinner.WorkItems.Wrappers;

namespace BreadWinner
{
    public class BatchedWorkItem : IWorkItem
    {
        private readonly IWorkItem _workItem;
        private readonly IWorkBatch _batch;

        public string Id => _workItem.Id;

        public BatchedWorkItem(IWorkBatch batch, Func<CancellationToken, WorkItemResult> doWork, string workId = null)
            : this(batch, new DoWorkWrapper(doWork, workId))
        {
        }

        public BatchedWorkItem(IWorkBatch batch, IWorkItem workItem)
        {
            _workItem = workItem;
            _batch = batch;
        }

        public WorkItemResult Do(CancellationToken cancellationToken)
        {
            var result = _workItem.Do(cancellationToken);

            _batch.DoFinally(result, cancellationToken);

            return new WorkItemResult(WorkStatus.Successful);
        }
    }
}