using System;
using System.Threading;

namespace BreadWinner.WorkItems.Wrappers
{
    internal class DoWorkWrapper : IWorkItem
    {
        private readonly Func<CancellationToken, WorkItemResult> _doWork;
        public string Id { get; }

        public DoWorkWrapper(Func<CancellationToken, WorkItemResult> doWork, string workId)
        {
            _doWork = doWork ?? throw new ArgumentNullException(nameof(doWork));
            Id = workId ?? Guid.NewGuid().ToString();
        }

        public WorkItemResult Do(CancellationToken cancellationToken)
        {
            return _doWork(cancellationToken);
        }
    }
}