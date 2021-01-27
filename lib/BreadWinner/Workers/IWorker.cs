using System.Threading;

namespace BreadWinner
{
    public interface IWorker
    {
        void Start(CancellationToken cancellationToken);

        bool IsAlive { get; }
    }
}