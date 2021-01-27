using System;
using System.Net;
using System.Threading;
using SamplesShared;

namespace ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 1000;

            var tokenSource = new CancellationTokenSource();

            WorkerPoolExample.StartPool(
                bool.Parse(args[0]),
                new TimeSpan(0, 0, 0, int.Parse(args[1])),
                new TimeSpan(0, 0, 0, int.Parse(args[2])),
                int.Parse(args[3]),
                int.Parse(args[4]),
                tokenSource.Token);

            FormattedConsole.WriteLine("Pool started correctly!");

            while (Console.ReadKey().KeyChar != 'q')
            {
            }

            tokenSource.Cancel();
        }
    }
}
