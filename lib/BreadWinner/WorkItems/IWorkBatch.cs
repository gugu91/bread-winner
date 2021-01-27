using System.Threading;

namespace BreadWinner
{
    public interface IWorkBatch
    {
        string Id { get; }

        void DoFinally(WorkItemResult result, CancellationToken cancellationToken);
    }
}