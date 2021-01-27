using System.Collections.Generic;
using System.Threading;

namespace BreadWinner
{
    public interface IWorkItemFactory
    {
        IEnumerable<IWorkItem> CreateStartupWorkItems(CancellationToken cancellationToken);

        IEnumerable<IWorkItem> CreateWorkItems(CancellationToken cancellationToken);
    }
}