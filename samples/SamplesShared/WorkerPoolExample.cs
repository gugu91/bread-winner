using System;
using System.Threading;
using BreadWinner;
using BreadWinner.Threading;
using SamplesShared.BlobWatcher;
using SamplesShared.DummyExample;

namespace SamplesShared
{
    public class WorkerPoolExample
    {
        public static void StartPool(
            bool dummy,
            TimeSpan workArrivedSchedule, 
            TimeSpan producerSchedule, 
            int consumers,
            int consecutiveBatchesAvailable,
            CancellationToken cancellationToken)
        {
            var workAvailableRepo = new WorkAvailableRepo(consecutiveBatchesAvailable);

            var workItemFactory = GetWorkFactory(dummy, workAvailableRepo);

            using (var startupBarrier = new StartupBarrier())
            {
                var workerPoolBuilder = new WorkerPoolBuilder(startupBarrier)
                    .WithScheduledProducer(producerSchedule, workItemFactory)
                    .WithNConsumers(consumers);

                if (dummy)
                {
                    workerPoolBuilder.WithScheduledJob(workArrivedSchedule, token => { workAvailableRepo.Reset(); });
                }

                workerPoolBuilder.Build().Start(cancellationToken);

                startupBarrier.Wait(cancellationToken);
            }
        }

        private static IBatchedWorkItemFactory GetWorkFactory(
            bool dummy, WorkAvailableRepo workAvailableRepo)
        {
            if (!dummy)
            {
                return new BlobWatcherWorkItemFactory();
            }

            return new DummyWorkItemFactory(workAvailableRepo);
        }
    }
}