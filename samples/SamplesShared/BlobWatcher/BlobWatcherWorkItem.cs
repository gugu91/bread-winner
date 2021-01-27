using System;
using System.Configuration;
using System.IO;
using System.Threading;
using BreadWinner;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SamplesShared.BlobWatcher
{
    // TODO: move in its own project
    public class BlobWatcherWorkItem : IWorkItem
    {
        private readonly IWorkBatch _batch;
        public string Id { get; }

        public BlobWatcherWorkItem(string blobUri, IWorkBatch batch)
        {
            _batch = batch;
            Id = blobUri;
        }

        public WorkItemResult Do(CancellationToken cancellationToken)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(
                    ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"]);
                var blockBlob = GetBlobReference(storageAccount, Id);

                var fileName = Path.GetFileName(blockBlob.Name);
                blockBlob.DownloadToFile(fileName, FileMode.Create);

                return new WorkItemResult(WorkStatus.Successful, Id);
            }
            catch (Exception e)
            {
                FormattedConsole.WriteLine($"Work Item {Id} of {_batch.Id} failed, exception {e.Message}");
                return new WorkItemResult(WorkStatus.Failed, Id);
            }
        }

        private static ICloudBlob GetBlobReference(CloudStorageAccount storageAccount, string uri)
        {
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blockBlob = blobClient.GetBlobReferenceFromServer(new Uri(uri));

            return blockBlob;
        }
    }
}