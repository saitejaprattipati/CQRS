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
using Author.Core.Services.EventBus;
using Author.Core.Services.EventBus.Interfaces;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Author.Command.Service
{
    public class IntegrationEventPublisherService : IIntegrationEventPublisherServiceService
    {

        const string ServiceBusConnectionString = "Endpoint=sb://taxathandpoc.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=NvQ9K2lU5nIb1Sz7lfWS/2QgFzxIgGV6xFlkyj656Xk=";
        const string QueueName = "taxathandqueue";
        static IQueueClient queueClient;
        private readonly IEventBus _bus;



        public IntegrationEventPublisherService(IEventBus bus)
        {
            _bus = bus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            try
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


                _bus.Publish(evt);
            }
            catch (Exception ex)
            {
            }
        }
        private static string createToken(string resourceUri, string keyName, string key)
        {
            TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var week = 60 * 60 * 24 * 7;
            var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + week);
            string stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            var sasToken = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}", HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry, keyName);
            return sasToken;
        }

    }


    /// <summary>
    /// publish QCResource update event service
    /// </summary>
    public interface IIntegrationEventPublisherServiceService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
