using System;
using System.Collections.Generic;
using System.Threading;
using BreadWinner.Threading;

namespace BreadWinner
{
    internal class ScheduledProducer : AbstractProducer
    {
        private readonly TimeSpan _timespan;
        private readonly Func<CancellationToken, IEnumerable<IWorkItem>> _startupWorkFactoryMethod;
        private readonly Func<CancellationToken, IEnumerable<IWorkItem>> _workItemFactoryMethod;

        internal ScheduledProducer(TimeSpan timespan,
            Func<CancellationToken, IEnumerable<IWorkItem>> startupWorkFactoryMethod,
            Func<CancellationToken, IEnumerable<IWorkItem>> workFactoryMethod,
            IStartupBarrier startupBarrier = null)
            : base(startupBarrier)
        {
            _timespan = timespan;
            _startupWorkFactoryMethod = startupWorkFactoryMethod;
            _workItemFactoryMethod = workFactoryMethod ?? throw new ArgumentNullException(nameof(workFactoryMethod));
        }

        protected override void Startup(Action<IEnumerable<IWorkItem>, CancellationToken> addWork, CancellationToken cancellationToken)
        {
            if (_startupWorkFactoryMethod == null)
            {
                return;
            }

            addWork(_startupWorkFactoryMethod.Invoke(cancellationToken), cancellationToken);
        }

        protected override void QueueWork(Action<IEnumerable<IWorkItem>, CancellationToken> addWork, CancellationToken cancellationToken)
        {
            addWork(_workItemFactoryMethod.Invoke(cancellationToken), cancellationToken);
        }

        protected override bool WaitForWorkOrCancellation(CancellationToken cancellationToken)
        {
            return cancellationToken.WaitHandle.WaitOne(_timespan);
        }
    }
}