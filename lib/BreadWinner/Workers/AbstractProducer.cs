using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using BreadWinner.Threading;

namespace BreadWinner
{
    public abstract class AbstractProducer : IWorker
    {
        private readonly IThreadWrapper _worker;
        public bool IsAlive => _worker.IsAlive;
        internal Action<IEnumerable<IWorkItem>, CancellationToken> AddWork { get; set; }

        [ExcludeFromCodeCoverage]
        protected AbstractProducer(IStartupBarrier startupBarrier = null) : this(new ThreadWrapper(), startupBarrier)
        {
        }

        internal AbstractProducer(IThreadWrapper worker, IStartupBarrier startupBarrier = null)
        {
            _worker = worker;
            _worker.Setup(Loop, 
                cancellationToken =>
                {
                    Startup(AddWork, cancellationToken);

                    startupBarrier?.Wait(cancellationToken);
                });
        }

        public void Start(CancellationToken cancellationToken)
        {
            _worker.Start(cancellationToken);
        }

        protected abstract void Startup(Action<IEnumerable<IWorkItem>, CancellationToken> addWork, CancellationToken cancellationToken);

        protected abstract void QueueWork(Action<IEnumerable<IWorkItem>, CancellationToken> addWork, CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>true if cancellation was requested</returns>
        protected abstract bool WaitForWorkOrCancellation(CancellationToken cancellationToken);

        private void Loop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (WaitForWorkOrCancellation(cancellationToken))
                    {
                        break;
                    }

                    QueueWork(AddWork, cancellationToken);
                }
                catch (Exception ex)
                {
                    // Exceptions are also caught at lower  level, but have to be caught here not to exit the loop
                    Trace.WriteLine(
                        $"BreadWinner - AbstractProducer - Wait or Queue Work - Unhandled exception in implementor: {ex}");
                    if (ConfigHelper.ThrowExceptions)
                    {
                        throw;
                    }
                }
            }
        }
    }
}