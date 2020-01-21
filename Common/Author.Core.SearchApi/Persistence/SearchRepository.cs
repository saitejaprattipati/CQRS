using Author.Core.SearchApi.Domain;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;

namespace Author.Core.SearchApi.Persistence
{
    public class SearchRepository :ISearchRepository
    {
        SearchIndexClient _indexClient;
        SearchServiceClient _serviceClient;
        public SearchRepository(SearchServiceClient serviceClient , SearchIndexClient indexClient)
        {
            _indexClient = indexClient;
            _serviceClient = serviceClient;
        }
        public void CreateIndex(string indexName)
        {            
            if (_serviceClient.Indexes.ExistsAsync(indexName).Result)
            {
                _serviceClient.Indexes.DeleteAsync(indexName);
            }            
            var definition = new Index()
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<publicindex>()
            };
            _serviceClient.Indexes.Create(definition);
        }
        public void DeleteIndex(string indexName)
        {
            if (_serviceClient.Indexes.ExistsAsync(indexName).Result)
            {
                _serviceClient.Indexes.DeleteAsync(indexName);
            }
        }
        public void UploadIndexData(List<publicindex> data)
        {
            var batch = IndexBatch.Upload(data);

            try
            {
                _indexClient.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
            }
        }
    }
}
