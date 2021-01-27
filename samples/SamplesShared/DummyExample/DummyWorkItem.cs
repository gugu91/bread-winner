using System;
using System.Runtime.Remoting.Metadata;
using System.Threading;
using BreadWinner;

namespace SamplesShared.DummyExample
{
    public class DummyWorkItem : IWorkItem
    {
        public static int _seed = 10;

        public static int Seed => Interlocked.Add(ref _seed, 10);

        public string Id { get; }

        public DummyWorkItem(string id)
        {
            Id = id;
        }

        public WorkItemResult Do(CancellationToken cancellationToken)
        {
            FormattedConsole.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} consuming {Id}");

            var rand = new Random(Seed);
            var timeTaken = rand.Next(0, 9);
            cancellationToken.WaitHandle.WaitOne(timeTaken * 1000);

            FormattedConsole.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} consumed {Id}, time taken: {timeTaken}");

            return new WorkItemResult(WorkStatus.Successful);
        }
    }
}