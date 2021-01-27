using System;
using System.Collections;
using System.Threading;
using BreadWinner.Threading;
using BreadWinner.WorkItems.Wrappers;

namespace BreadWinner
{
    public class WorkBatchFactory : IWorkBatchFactory
    {
        private readonly IStartupBarrier _startupBarrier;
        private bool _startupCreated = false;

        public WorkBatchFactory(IStartupBarrier startupBarrier = null)
        {
            _startupBarrier = startupBarrier;
        }

        public IWorkBatch Create(int workBatchSize,
            Action<WorkItemResult, CancellationToken> finalizeBatch,
            string batchId = null,
            IEnumerable preComputedResults = null,
            bool startup = false)
        {
            return Create(workBatchSize, new FinalizeBatchWrapper(finalizeBatch, batchId), preComputedResults, startup);
        }

        public IWorkBatch Create(int workBatchSize, IWorkBatch yourBatch, IEnumerable preComputedResults = null,
            bool startup = false)
        {
            var batch = new WorkBatch(workBatchSize, yourBatch, preComputedResults);
            if (!startup)
            {
                return batch;
            }

            if (_startupCreated)
            {
                throw new ApplicationException("You can only have one startup batch");
            }

            _startupCreated = true;

            return new StartupWorkBatch(batch, _startupBarrier);
        }
    }
}