using System;
using System.Collections.Generic;
using System.Threading;
using BreadWinner.Threading;

namespace BreadWinner.UnitTests.TestDoubles
{
    internal class TestProducer : AbstractProducer
    {
        internal CancellationToken StartupCalledWith { get; private set; }
        internal Tuple<Action<IEnumerable<IWorkItem>, CancellationToken>, CancellationToken> QueueWorkCalledWith { get; private set; }
        internal CancellationToken WaitForWorkOrCancellationCalledWith { get; private set; }
        internal Func<CancellationToken, bool> WaitAction { get; private set; }
        internal Action<CancellationToken> QueueAction { get; private set; }


        internal TestProducer(IThreadWrapper threadWrapper) : base(threadWrapper)
        {
        }

        protected override void Startup(Action<IEnumerable<IWorkItem>, CancellationToken> addWork, CancellationToken cancellationToken)
        {
            StartupCalledWith = cancellationToken;
        }

        protected override void QueueWork(Action<IEnumerable<IWorkItem>, CancellationToken> addWork,
            CancellationToken cancellationToken)
        {
            QueueWorkCalledWith = new Tuple<Action<IEnumerable<IWorkItem>, CancellationToken>, CancellationToken>(addWork, cancellationToken);
            
            QueueAction?.Invoke(cancellationToken);
        }

        protected override bool WaitForWorkOrCancellation(CancellationToken cancellationToken)
        {
            WaitForWorkOrCancellationCalledWith = cancellationToken;

            return WaitAction?.Invoke(cancellationToken) ?? false;
        }

        internal void SetupWaitForWorkOrCancellation(Func<CancellationToken, bool> waitAction)
        {
            WaitAction = waitAction;
        }

        internal void SetupQueueWork(Action<CancellationToken> queueAction)
        {
            QueueAction = queueAction;
        }
    }
}