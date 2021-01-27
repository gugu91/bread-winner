using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;

namespace BreadWinner
{
    internal class WorkBatch : IWorkBatch
    {
        private int _workBatchSize;
        private readonly IWorkBatch _batch;
        private int _status;
        private readonly ConcurrentBag<object> _results;

        public string Id => _batch.Id;

        private WorkStatus Status
        {
            get => (WorkStatus) _status;
            set => Interlocked.Exchange(ref _status, (int) value);
        }

        internal WorkBatch(int workBatchSize, IWorkBatch batch, IEnumerable preComputedResults = null)
        {
            if (workBatchSize < 1)
            {
                throw new ArgumentException(nameof(workBatchSize));
            }

            _batch = batch ?? throw new ArgumentNullException(nameof(batch));
            _workBatchSize = workBatchSize;
            _results = new ConcurrentBag<object>();
            Status = WorkStatus.Successful;
            LoadPreComputedResults(preComputedResults);
        }

        public void DoFinally(WorkItemResult result, CancellationToken cancellationToken)
        {
            var batchDone = IsBatchDone(); // Has to be done first

            UpdateStatusAndStoreResults(result);

            if (batchDone)
            {
                _batch.DoFinally(new WorkItemResult(Status, _results), cancellationToken);
            }
        }

        private bool IsBatchDone()
        {
            // Do not change, thread safe
            if (_workBatchSize <= 0)
            {
                throw new ApplicationException("Batch completed");
            }

            var batchDone = Interlocked.Decrement(ref _workBatchSize) == 0;

            return batchDone;
        }

        private void UpdateStatusAndStoreResults(WorkItemResult result)
        {
            if (result.Status == WorkStatus.Failed && Status == WorkStatus.Successful)
            {
                Status = WorkStatus.Failed;
            }

            if (result.Data == null)
            {
                return;    
            }

            foreach (var data in result.Data)
            {
                _results.Add(data);
            }
        }

        private void LoadPreComputedResults(IEnumerable preComputedResults)
        {
            if (preComputedResults == null)
            {
                return;
            }

            foreach (var o in preComputedResults)
            {
                _results.Add(o);
            }
        }
    }
}
