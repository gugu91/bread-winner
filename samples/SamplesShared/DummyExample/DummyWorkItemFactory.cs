using System;
using System.Linq;
using System.Threading;
using BreadWinner;

namespace SamplesShared.DummyExample
{
    public class DummyWorkItemFactory : IWorkItemFactory
    {
        private readonly WorkAvailableRepo _workAvailableRepo;

        public DummyWorkItemFactory(WorkAvailableRepo workAvailableRepo)
        {
            _workAvailableRepo = workAvailableRepo;
        }

        public IWorkItem[] Startup(CancellationToken cancellationToken, ManualResetEvent started = null)
        {
            CloudConsole.WriteLine("Producer startup");
            return GetWorkItems(started);
        }

        public IWorkItem[] CreateWorkItems(CancellationToken cancellationToken)
        {
            if (_workAvailableRepo.WorkAvailableId() < 0)
            {
                return null;
            }

            return GetWorkItems();
        }

        private static IWorkItem[] GetWorkItems(EventWaitHandle started = null)
        {
            var rand = new Random();
            var workBatch = new WorkBatch(3, new DummyWorkBatch(started));

            var workItems = new[]
            {
                new BatchedWorkItem(workBatch, new DummyWorkItem(rand.Next())),
                new BatchedWorkItem(workBatch, new DummyWorkItem(rand.Next())),
                new BatchedWorkItem(workBatch, new DummyWorkItem(rand.Next()))
            };

            CloudConsole.WriteLine(
                $"Producer {Thread.CurrentThread.ManagedThreadId} has created " +
                $"{workItems[0].Id}, {workItems[1].Id}, {workItems[2].Id}");

            return workItems.Cast<IWorkItem>().ToArray();
        }
    }
}