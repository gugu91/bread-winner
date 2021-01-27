using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BreadWinner;
using SamplesShared.BlobExample;

namespace SamplesShared.BlobWatcher
{
    // TODO: move in its own project
    public class BlobWatcherWorkItemFactory : IBatchedWorkItemFactory
    {
        public IEnumerable<BatchedWorkItem> CreateStartupWorkItems(IWorkBatchFactory workBatchFactory, CancellationToken cancellationToken)
        {
            // Load everything at startup
            return GetWorkItems(BlobHelper.GetAllFilesInContainerWithPath("*"), workBatchFactory);
        }

        public IEnumerable<BatchedWorkItem> CreateWorkItems(IWorkBatchFactory workBatchFactory, CancellationToken cancellationToken)
        {
            var blobs = BlobHelper.GetAllFilesInContainerWithPath(string.Empty);
            var localFile = Directory.GetFiles(".", "*", SearchOption.AllDirectories);

            var toDownload = blobs.Where(blob => NotDowloaded(blob, localFile)).ToList();

            return GetWorkItems(toDownload, workBatchFactory);
        }

        private static IEnumerable<BatchedWorkItem> GetWorkItems(IReadOnlyCollection<Uri> fileLocations, IWorkBatchFactory workBatchFactory, string batchId = null)
        {
            var batch = workBatchFactory.Create(fileLocations.Count, new BlobWatcherWorkBatch(batchId, ProcessResults));

            FormattedConsole.WriteLine($"Created batch {batch.Id} with {fileLocations.Count} blobs");

            return fileLocations
                .Select(uri => new BatchedWorkItem(batch, new BlobWatcherWorkItem(uri.AbsoluteUri, batch)));
        }

        private static bool NotDowloaded(Uri dir, string[] local)
        {
            // TODO: do something

            return true;
        }

        private static void ProcessResults(WorkItemResult batchResult, int batchId)
        {
            // TODO: write success file
        }
    }
}