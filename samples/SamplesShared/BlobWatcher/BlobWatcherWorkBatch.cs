using System;
using System.Threading;
using BreadWinner;

namespace SamplesShared.BlobWatcher
{
    // TODO: move in its own project
    internal class BlobWatcherWorkBatch : IWorkBatch
    {
        private readonly Action<WorkItemResult, int> _processResults;

        public string Id { get; }

        public BlobWatcherWorkBatch(string id, Action<WorkItemResult, int> processResults)
        {
            Id = id ?? Guid.NewGuid().ToString();
            _processResults = processResults;
        }

        public void DoFinally(WorkItemResult result, CancellationToken cancellationToken)
        {
            try
            {
                FormattedConsole.WriteLine($"{Id} dofinally started");

                _processResults(result, int.Parse(Id));

                FormattedConsole.WriteLine($"Batch {Id} done");
            }
            catch (Exception e)
            {
                FormattedConsole.WriteLine($"Batch {Id} : exception in dofinally, {e}");
            }
        }
    }
}