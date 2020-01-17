using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Core.Services.Persistence.CosmosDB
{
    public class CosmosDBContext
    {
        public DocumentClient Client { get; private set; }

        private CosmosDBConnectionSettings Connection { get; }
        private readonly int _defaultPagingCount = 100;

        public Uri GetCollectionURI(string colectionName)
        {
            return UriFactory.CreateDocumentCollectionUri(Connection.DatabaseName, colectionName);
        }

        public CosmosDBContext()
        {
            Connection = ReadConfiguration();
            this.InitializeAsync(Connection.EndpointURL, Connection.AccessKey, Connection.DatabaseName).Wait();
        }
        private CosmosDBConnectionSettings ReadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            var config = builder.Build();
            var connectionInfo = new CosmosDBConnectionSettings
            {
                EndpointURL = config["CosmosDBEndpoint"],
                DatabaseName = config["CosmosDBName"],
                AccessKey = config["CosmosDBAccessKey"]
            };

            if (string.IsNullOrEmpty(connectionInfo.EndpointURL))
            {
                builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json");
                config = builder.Build();

                connectionInfo = new CosmosDBConnectionSettings
                {
                    EndpointURL = config["CosmosDBEndpoint"],
                    DatabaseName = config["CosmosDBName"],
                    AccessKey = config["CosmosDBAccessKey"]
                };
            }

            return connectionInfo;
        }
        private async Task InitializeAsync(string endpointUrl, string accessKey, string databaseName)
        {
            ConnectionPolicy connectionPolicy = new ConnectionPolicy
            {
                ConnectionMode = ConnectionMode.Gateway,
                ConnectionProtocol = Protocol.Https
            };
            try
            {
                this.Client = new DocumentClient(new Uri(endpointUrl), accessKey);
                await this.Client.CreateDatabaseIfNotExistsAsync(new Microsoft.Azure.Documents.Database() { Id = databaseName });
            }
            catch
            {
                // TO DO: Ex handling
            }
        }
        public async Task<Document> Get(string partitionKey, string collectionName, string documentId)
        {
            try
            {
                var requestOptions = new RequestOptions();

                var response =(await this.Client.ReadDocumentAsync(
                    UriFactory.CreateDocumentUri(Connection.DatabaseName, collectionName, documentId),
                new RequestOptions { PartitionKey = new PartitionKey(partitionKey) }));
               
                return response.Resource;
            }
            catch (DocumentClientException ex)
            {
                // do something
            }

            return null;
        }

        public PagingBase<Document> GetAll(string collection, int pageNumber = 1, int pageSize = 1)
        {
            var client = this.Client;
            var docs = client.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(Connection.DatabaseName, collection), new FeedOptions { EnableCrossPartitionQuery = true, MaxDegreeOfParallelism = 10, MaxBufferedItemCount = 100 })
                      .OrderByDescending(d => d.Timestamp);
            var records = docs.ToList();
            var totalRecordCount = records.Count(); pageSize = totalRecordCount;
            return GetRecordsByPaging<Document>(records, pageNumber, pageSize, totalRecordCount);
        }

        public PagingBase<Document> Get(string collection, int pageNumber = 1, int pageSize = 100)
        {
            var client = this.Client;
            var totalRecordCount = client.CreateDocumentQuery<int>(
                UriFactory.CreateDocumentCollectionUri(Connection.DatabaseName, collection), "Select VALUE COUNT(c) from c", new FeedOptions { EnableCrossPartitionQuery = true , MaxDegreeOfParallelism = 10, MaxBufferedItemCount = 100 }
                ).AsEnumerable().First();


            var docs = client.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(Connection.DatabaseName, collection), new FeedOptions { EnableCrossPartitionQuery = true, MaxDegreeOfParallelism = 10, MaxBufferedItemCount = 100 })
                      .OrderByDescending(d => d.Timestamp).Take(pageNumber * pageSize);
            var records = docs.ToList();

            return GetRecordsByPaging<Document>(records, pageNumber, pageSize, totalRecordCount);
        }
        private PagingBase<T> GetRecordsByPaging<T>(List<T> result, int pageNumber, int pageSize, int totalRecordCount)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new PagingBase<T>
                {
                    Records = result.Take(_defaultPagingCount).AsEnumerable()
                };
            }

            int totalpages = (int)Math.Ceiling(totalRecordCount / (double)pageSize);
            if (pageNumber > totalpages)
            {
                pageNumber = totalpages;
            }
            PagingBase<T> pagingResult = new PagingBase<T>
            {
                Records = result.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable(),
                TotalCount = totalRecordCount,
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalPages = totalpages

            };

            return pagingResult;
        }
    }
    public class PagingBase<T>
    {

        public IEnumerable<T> Records { get; set; }
        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public int TotalPages { get; set; }
    }
}
