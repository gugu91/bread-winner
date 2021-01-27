using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BreadWinner.Threading;

namespace BreadWinner
{
    internal class WorkerPool : IWorkerPool
    {
        private readonly List<IWorker> _workers;
        private volatile bool _isStarted = false;

        public bool IsAlive
        {
            get
            {
                return _workers.All(x => x.IsAlive);
            }
        }

        internal WorkerPool()
        {
            _workers = new List<IWorker>();
        }

        public void Add(params IWorker[] workers)
        {
            if (_isStarted)
            {
                throw new ApplicationException("Pool started, cannot add workers");
            }

            _workers.AddRange(workers);
        }

        public void Start(CancellationToken cancellationToken)
        {
            if (!_isStarted && !cancellationToken.IsCancellationRequested)
            {
                _isStarted = true;
                foreach (var worker in _workers)
                {
                    worker.Start(cancellationToken);
                }
            }
            else
            {
                throw new ApplicationException("Pool already started");
            }
        }
    }
}