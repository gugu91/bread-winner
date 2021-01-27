using System.Collections.Generic;
using System.Threading;

namespace BreadWinner
{
    public interface IBatchedWorkItemFactory
    {
        IEnumerable<BatchedWorkItem> CreateStartupWorkItems(IWorkBatchFactory workBatchFactory, CancellationToken cancellationToken);

        IEnumerable<BatchedWorkItem> CreateWorkItems(IWorkBatchFactory workBatchFactory, CancellationToken cancellationToken);
    }
}