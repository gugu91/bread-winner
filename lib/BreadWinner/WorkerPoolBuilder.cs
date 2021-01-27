using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using BreadWinner.Threading;

namespace BreadWinner
{
    public class WorkerPoolBuilder
    {
        private readonly IWorkerPool _workerPool;
        private readonly BlockingCollection<IWorkItem> _workQueue;
        private readonly IStartupBarrier _barrier;

        public WorkerPoolBuilder(IStartupBarrier startupBarrier = null)
        {
            _barrier = startupBarrier;
            _workerPool = new WorkerPool();
            _workQueue = new BlockingCollection<IWorkItem>();
        }

        internal WorkerPoolBuilder(IWorkerPool workerPool, BlockingCollection<IWorkItem> workQueue)
        {
            _workerPool = workerPool;
            _workQueue = workQueue;
        }

        public WorkerPoolBuilder WithNConsumers(int n)
        {
            for (var i = 0; i < n; i++)
            {
                _workerPool.Add(new Consumer(TakeWork));
            }

            return this;
        }

        public WorkerPoolBuilder WithProducer(
            Func<AbstractProducer> factoryMethod)
        {
            var producer = factoryMethod();
            producer.AddWork = AddWork;

            _workerPool.Add(producer);

            return this;
        }

        public WorkerPoolBuilder WithScheduledProducer(TimeSpan schedule, IBatchedWorkItemFactory workItemFactory)
        {
            var workBatchFactory = new WorkBatchFactory(_barrier);
            return WithProducer(() =>
                new ScheduledProducer(
                    schedule, 
                    cancellationToken => workItemFactory.CreateStartupWorkItems(workBatchFactory, cancellationToken),
                    cancellationToken => workItemFactory.CreateWorkItems(workBatchFactory, cancellationToken),
                    _barrier));
        }

        public WorkerPoolBuilder WithScheduledProducer(TimeSpan schedule, IWorkItemFactory workItemFactory)
        {
            return WithProducer(() => 
                new ScheduledProducer(schedule, workItemFactory.CreateStartupWorkItems, workItemFactory.CreateWorkItems, _barrier));
        }

        public WorkerPoolBuilder WithScheduledProducer(
            TimeSpan schedule,
            Func<CancellationToken, IEnumerable<IWorkItem>> workFactoryMethod,
            Func<CancellationToken, IEnumerable<IWorkItem>> startupWorkFactoryMethod = null)
        {
            return WithProducer(() => 
                new ScheduledProducer(schedule, null, workFactoryMethod, _barrier));
        }

        public WorkerPoolBuilder WithScheduledJob(
            TimeSpan schedule,
            Action<CancellationToken> workItem,
            Action<CancellationToken> startupAction = null)
        {
            _workerPool.Add(new ScheduledJob(schedule, workItem, startupAction));

            return this;
        }

        public IWorker Build()
        {
            return _workerPool;
        }

        private void AddWork(IEnumerable<IWorkItem> workItems, CancellationToken cancellationToken)
        {
            if (workItems == null)
            {
                return;
            }

            foreach (var workItem in workItems)
            {
                _workQueue.Add(workItem, cancellationToken);
            }
        }

        private IWorkItem TakeWork(CancellationToken cancellationToken)
        {
            return _workQueue.Take(cancellationToken);
        }
    }
}
