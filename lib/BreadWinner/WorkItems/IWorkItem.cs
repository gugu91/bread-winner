using System.Threading;

namespace BreadWinner
{
    public interface IWorkItem
    {
        string Id { get; }

        WorkItemResult Do(CancellationToken cancellationToken);
    }
}