using Microsoft.Azure.Storage.Blob;
using System;

namespace Author.Core.Services.BlobStorage
{
    public interface IBlobConnection : IDisposable
    {
        CloudBlobClient BlobStorageConnectionStringBuilder { get; }  
    }
}
