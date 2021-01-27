using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using BreadWinner.Threading;

namespace BreadWinner
{
    internal class ScheduledJob : IWorker
    {
        private readonly TimeSpan _interval;
        private readonly Action<CancellationToken> _workItem;
        private readonly IThreadWrapper _worker;
        public bool IsAlive => _worker.IsAlive;

        [ExcludeFromCodeCoverage]
        internal ScheduledJob(
            TimeSpan interval, Action<CancellationToken> workItem, Action<CancellationToken> startupAction)
            : this(interval, workItem, startupAction, new ThreadWrapper())
        {
        }

        internal ScheduledJob(
            TimeSpan interval, Action<CancellationToken> workItem, Action<CancellationToken> startupAction, IThreadWrapper worker)
        {
            _interval = interval;
            _workItem = workItem;
            _worker = worker;
            _worker.Setup(Loop, startupAction);
        }

        public void Start(CancellationToken cancellationToken)
        {
            _worker.Start(cancellationToken);
        }

        private void Loop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.WaitHandle.WaitOne(_interval);
                try
                {
                    _workItem?.Invoke(cancellationToken);
                }
                catch (Exception ex)
                {
                    // Exceptions are also caught at lower level, but have to be caught here not to exit the loop
                    Trace.WriteLine($"BreadWinner - ScheduledJob - WorkItem - Unhandled exception in action: {ex}");
                    if (ConfigHelper.ThrowExceptions)
                    {
                        throw;
                    }
                }
            }
        }
    }
}