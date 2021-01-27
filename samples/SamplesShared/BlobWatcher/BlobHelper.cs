using System;
using System.Configuration;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SamplesShared.BlobExample
{
    // TODO: move in its own project
    public class BlobHelper
    {
        public static Uri[] GetAllFilesInContainerWithPath(string path)
        {
            var container = GetCloudBlobContainer();
            var blobs = container.ListBlobs(path, true, options: new BlobRequestOptions
            {
                MaximumExecutionTime = new TimeSpan(0, 0, 0, 20)
            }).ToArray();

            var blobsLocation = blobs.OfType<CloudBlockBlob>().Select(x => x.Uri).ToArray();

            return blobsLocation;
        }


        private static CloudBlobContainer GetCloudBlobContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"]);
            var containerName = ConfigurationManager.AppSettings["Azure.Storage.Container"];
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            return container;
        }
    }
}