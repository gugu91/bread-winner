using System;
using System.Threading;

namespace BreadWinner.Threading
{
    public interface IStartupBarrier : IDisposable
    {
        bool IsDisposed { get; }

        void Wait(CancellationToken cancellationToken);
    }
}