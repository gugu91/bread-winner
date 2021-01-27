using System.Threading;

namespace BreadWinner.UnitTests.WorkItems
{
    public class TestWorkBatch : IWorkBatch
    {
        public string Id { get; }

        public WorkItemResult Result { get; set; }
        public int Executions { get; set; }

        public TestWorkBatch(string id = null)
        {
            Id = id;
            Executions = 0;
        }

        public void DoFinally(WorkItemResult result, CancellationToken cancellationToken)
        {
            Result = result;
            Executions++;
        }
    }
}