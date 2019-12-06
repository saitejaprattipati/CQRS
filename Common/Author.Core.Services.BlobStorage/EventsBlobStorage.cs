using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Author.Core.Services.BlobStorage.Interfaces;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;

namespace Author.Core.Services.BlobStorage
{
    public class EventsBlobStorage : IEventStorage
    {
        private readonly IBlobConnection _blobConnection;
        private readonly ILogger<EventsBlobStorage> _logger;

        public EventsBlobStorage(IBlobConnection blobConnection, ILogger<EventsBlobStorage> logger)
        {
            _blobConnection = blobConnection;


            if (_blobConnection.Equals(""))
            {
                _logger.LogError($"BlobStorage connection was not established logging {blobConnection}");
                throw new ArgumentNullException(nameof(blobConnection));
            }
        }

        public async Task<List<string>> saveFile(ImageData imgData)
        {
            List<string> objURLs = new List<string>();
            string strContainerName = "png";
            CloudBlobContainer cloudBlobContainer = _blobConnection.BlobStorageConnectionStringBuilder.GetContainerReference(strContainerName);
            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
            if (imgData.JPGName != null && imgData.JPGData != null)
            {
                byte[] imageBytes = Convert.FromBase64String(imgData.JPGData);
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imgData.JPGName);

                //SAS Generation
                string SAS = GetBlobSasUri(cloudBlockBlob, null);
                cloudBlockBlob = new CloudBlockBlob(new Uri(SAS));

                cloudBlockBlob.Properties.ContentType = "jpg";
                await cloudBlockBlob.UploadFromByteArrayAsync(imageBytes, 0, imageBytes.Length);
                objURLs.Add(cloudBlockBlob.Uri.AbsoluteUri);
            }
            strContainerName = "svg";
            cloudBlobContainer = _blobConnection.BlobStorageConnectionStringBuilder.GetContainerReference(strContainerName);
            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
            if (imgData.SVGName != null && imgData.SVGData != null)
            {
                byte[] imageBytes = Convert.FromBase64String(imgData.SVGData);
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imgData.SVGName);

                //SAS Generation
                string SAS = GetBlobSasUri(cloudBlockBlob, null);
                cloudBlockBlob = new CloudBlockBlob(new Uri(SAS));

                cloudBlockBlob.Properties.ContentType = "svg";
                await cloudBlockBlob.UploadFromByteArrayAsync(imageBytes, 0, imageBytes.Length);
                objURLs.Add(cloudBlockBlob.Uri.AbsoluteUri);
            }
            return objURLs;
        }
        private static string GetBlobSasUri(CloudBlockBlob blob, string policyName = null)
        {
            string sasBlobToken;
            if (policyName == null)
            {
                SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy()
                {
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
                    Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
                };
                sasBlobToken = blob.GetSharedAccessSignature(adHocSAS, null, null, SharedAccessProtocol.HttpsOnly, null);

                Console.WriteLine("SAS for blob (ad hoc): {0}", sasBlobToken);
                Console.WriteLine();
            }
            else
            {
                sasBlobToken = blob.GetSharedAccessSignature(null, policyName);

                Console.WriteLine("SAS for blob (stored access policy): {0}", sasBlobToken);
                Console.WriteLine();
            }
            return blob.Uri + sasBlobToken;
        }
    }
    public class ImageData
    {
        public string JPGData { get; set; }
        public string JPGName { get; set; }
        public string SVGData { get; set; }
        public string SVGName { get; set; }
    }
}
