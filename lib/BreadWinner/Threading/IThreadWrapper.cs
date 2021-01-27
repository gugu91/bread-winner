using System;
using System.Threading;

namespace BreadWinner.Threading
{
    internal interface IThreadWrapper : IWorker
    {
        void Setup(Action<CancellationToken> loop, Action<CancellationToken> startup = null);
    }
}
