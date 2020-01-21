using Author.Core.SearchApi.Domain;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Author.Core.SearchApi.Persistence
{
    public class SearchRepository :ISearchRepository
    {
        SearchIndexClient _indexClient;
        public SearchRepository(SearchIndexClient indexClient)
        {
            _indexClient = indexClient;
        }
        public void CreateIndex(string indexName, SearchServiceClient serviceClient)
        {            
            if (serviceClient.Indexes.ExistsAsync(indexName).Result)
            {
                serviceClient.Indexes.DeleteAsync(indexName);
            }            
            var definition = new Index()
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<PublicIndex>()
            };
            serviceClient.Indexes.Create(definition);
        }
    }
}
