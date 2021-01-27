using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace BreadWinner.Threading
{
    [ExcludeFromCodeCoverage]
    public class StartupBarrier : IStartupBarrier
    {
        private Barrier _barrier;
        private bool _waited;

        public bool IsDisposed => _barrier == null;

        public StartupBarrier()
        {
            _barrier = new Barrier(3);
        }

        public void Wait(CancellationToken cancellationToken)
        {
            _barrier.SignalAndWait(cancellationToken);
            _waited = true;
        }

        public void Dispose()
        {
            if (!_waited)
            {
                throw new ApplicationException("Please wait on barrier");    
            }

            _barrier?.Dispose();
            _barrier = null;
        }
    }
}
