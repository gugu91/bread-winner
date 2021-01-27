using System;
using System.Threading;
using BreadWinner;

namespace SamplesShared.DummyExample
{
    internal class DummyWorkBatch : IWorkBatch
    {
        public string Id { get; }

        public DummyWorkBatch()
        {
            Id = Guid.NewGuid().ToString();
        }

        public void DoFinally(WorkItemResult result, CancellationToken cancellationToken)
        {
            FormattedConsole.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} finalized batch in work item {Id}");
        }
    }
}