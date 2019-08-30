using Microsoft.Azure.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure.Storage.Blob;

namespace Author.Core.Services.BlobStorage
{
    public class BlobConnection : IBlobConnection
    {
        bool _disposed;
        public static CloudBlobClient cloudBlobClient = null;
       // string storageConnectionString = "DefaultEndpointsProtocol=http;AccountName=taxathandsample;AccountKey=13EjvMoEDxYIELZSqzZjTeuW1PxnjCISL/9SEGhYCBSP4AMnKthwsRJCOUtKM0hxsz4OHZiRuLz0nt4+D4azhg==;EndpointSuffix=core.windows.net";
        public BlobConnection(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
                cloudBlobClient = storageAccount.CreateCloudBlobClient();
            else
                throw new ArgumentNullException(nameof(storageAccount));
        }
        public CloudBlobClient BlobStorageConnectionStringBuilder => cloudBlobClient;
        //private static async Task ProcessAsync()
        //{
        //    string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=pksource;AccountKey=GPugIsjUcFr6sKCOLRFr8yGD8JdR3X0ZZKK+egdR3Lv2xQWH8Nv6MWW7jWQMlyRy0GwNWFyOhjQTUpKzFldkXw==;EndpointSuffix=core.windows.net";

        //    CloudStorageAccount storageAccount;
        //    if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
        //    {

        //        cloudBlobClient = storageAccount.CreateCloudBlobClient();
        //    }
        //    else
        //    {
        //        Console.WriteLine(
        //            "A connection string has not been defined in the system environment variables. " +
        //            "Add an environment variable named 'STORAGE_CONNECTION_STRING' with your storage " +
        //            "connection string as a value.");
        //        Console.WriteLine("Press any key to exit the application.");
        //        Console.ReadLine();
        //    }
        //}
        //public async Task<string> saveFile(string fileData, string fileName)
        //{
        //    CloudBlobContainer cloudBlobContainer =
        //              cloudBlobClient.GetContainerReference("taxathandcontents");
        //    await cloudBlobContainer.CreateIfNotExistsAsync();
        //    BlobContainerPermissions permissions = new BlobContainerPermissions
        //    {
        //        PublicAccess = BlobContainerPublicAccessType.Blob
        //    };
        //    await cloudBlobContainer.SetPermissionsAsync(permissions);
        //    CloudBlobContainer container = cloudBlobClient.GetContainerReference("png"); container.Exists();
        //    byte[] imageBytes = Convert.FromBase64String(fileData);
        //    CloudBlockBlob blob = container.GetBlockBlobReference("png");
        //    blob.Properties.ContentType = "Jpeg";
        //    blob.UploadFromByteArray(imageBytes, 0, imageBytes.Length);
        //    return blob.StorageUri.PrimaryUri.AbsoluteUri;
        //}

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
        }
    }
}
