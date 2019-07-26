using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Author.Command.Events;
using Author.Core.Services.BlobStorage;
using Author.Core.Services.BlobStorage.Interfaces;
using Author.Core.Services.EventBus;
using Author.Core.Services.EventBus.Interfaces;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Author.Command.Domain.Models;

namespace Author.Command.Service
{
   public class IntegrationEventBlobService : IIntegrationEventBlobService
    {
        private readonly IEventStorage _blob;
        public IntegrationEventBlobService(IEventStorage blob)
        {
            _blob = blob;
        }
        public async Task<List<string>> PublishThroughBlobStorageAsync(Domain.Models.ImageData imageData)
        {
            //queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            //var jsonMessage = JsonConvert.SerializeObject(evt);
            //var message = new Message
            //{
            //    MessageId = Guid.NewGuid().ToString(),
            //    Body = Encoding.UTF8.GetBytes(jsonMessage),
            //    ContentType = "application/json",
            //    Label = "ArticleCreated",
            //};
            //// Send messages.
            //await queueClient.SendAsync(message);
            //await queueClient.CloseAsync();
            Core.Services.BlobStorage.ImageData objImageData = new Core.Services.BlobStorage.ImageData()
            {
                JPGData= imageData.JPGData,
                JPGName = imageData.JPGName,
                SVGData = imageData.SVGData,
                SVGName = imageData.SVGName
            };

              List<string> url=await _blob.saveFile(objImageData);
                return url;
        }
    }
    public interface IIntegrationEventBlobService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        Task<List<string>> PublishThroughBlobStorageAsync(Domain.Models.ImageData imageData);
    }
}
