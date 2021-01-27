using System;
using System.Collections;
using System.Threading;

namespace BreadWinner
{
    public interface IWorkBatchFactory
    {
        IWorkBatch Create(int workBatchSize,
            Action<WorkItemResult, CancellationToken> finalizeBatch,
            string batchId = null,
            IEnumerable preComputedResults = null,
            bool startup = false);

        IWorkBatch Create(int workBatchSize, IWorkBatch yourBatch, IEnumerable preComputedResults = null, bool startup = false);
    }
}
