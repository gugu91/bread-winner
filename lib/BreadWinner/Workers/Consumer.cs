using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using BreadWinner.Threading;

namespace BreadWinner
{
    internal class Consumer : IWorker
    {
        private readonly Func<CancellationToken, IWorkItem> _takeWork;
        private readonly IThreadWrapper _threadWrapper;
        public bool IsAlive => _threadWrapper.IsAlive;

        [ExcludeFromCodeCoverage]
        internal Consumer(Func<CancellationToken, IWorkItem> takeWork) : this(takeWork, new ThreadWrapper())
        {
        }

        internal Consumer(Func<CancellationToken, IWorkItem> takeWork, IThreadWrapper threadWrapper)
        {
            _takeWork = takeWork;
            _threadWrapper = threadWrapper;
            _threadWrapper.Setup(Loop);
        }

        public void Start(CancellationToken cancellationToken)
        {
            _threadWrapper.Start(cancellationToken);
        }

        private void Loop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = _takeWork(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                try
                {
                    workItem.Do(cancellationToken);
                }
                catch (Exception ex)
                {
                    // Exceptions are also caught at lower, but ave to be caught here not to exit the loop
                    Trace.WriteLine(
                        $"BreadWinner - Consumer - WorkItem Do - Unhandled exception in implementor: {ex}");
                    if (ConfigHelper.ThrowExceptions)
                    {
                        throw;
                    }
                }
            }
        }
    }
}