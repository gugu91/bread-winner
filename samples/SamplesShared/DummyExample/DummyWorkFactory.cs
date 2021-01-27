using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BreadWinner;

namespace SamplesShared.DummyExample
{
    public class DummyWorkItemFactory : IBatchedWorkItemFactory
    {
        private readonly WorkAvailableRepo _workAvailableRepo;
        private readonly Random _rand;

        private static int ProducerId => Thread.CurrentThread.ManagedThreadId;

        public DummyWorkItemFactory(WorkAvailableRepo workAvailableRepo)
        {
            _workAvailableRepo = workAvailableRepo;
            _rand = new Random();
        }

        public IEnumerable<BatchedWorkItem>CreateStartupWorkItems(IWorkBatchFactory workBatchFactory, CancellationToken cancellationToken)
        {
            FormattedConsole.WriteLine($"Producer {ProducerId} - Startup");
            return GetWorkItems(workBatchFactory, true);
        }

        public IEnumerable<BatchedWorkItem> CreateWorkItems(IWorkBatchFactory workBatchFactory, CancellationToken cancellationToken)
        {
            FormattedConsole.WriteLine($"Producer {ProducerId} - Checking for work...");

            while (_workAvailableRepo.WorkAvailable())
            {
                foreach (var workItem in GetWorkItems(workBatchFactory))
                {
                    yield return workItem;
                }
            }
        }

        private IEnumerable<BatchedWorkItem> GetWorkItems(IWorkBatchFactory workBatchFactory, bool startup = false)
        {
            var workBatch = workBatchFactory.Create(3, new DummyWorkBatch(), startup: startup);

            var workItems = new List<BatchedWorkItem>();

            for (var i = 0; i < 3; i++)
            {
                workItems.Add(new BatchedWorkItem(workBatch, new DummyWorkItem(_rand.Next(0, 100).ToString())));
            }

            FormattedConsole.WriteLine($"Producer {ProducerId} has created {workItems[0].Id}, {workItems[1].Id}, {workItems[2].Id}");

            return workItems;
        }
    }
}