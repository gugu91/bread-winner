using System.Collections;

namespace BreadWinner
{
    public class WorkItemResult
    {
        public WorkStatus Status { get; }

        public IEnumerable Data { get; }

        public WorkItemResult(WorkStatus status, IEnumerable data = null)
        {
            Status = status;
            Data = data;
        }
    }
}
