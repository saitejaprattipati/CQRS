using Author.Core.SearchApi.Domain;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;




using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
//using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
//using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Author.Core.SearchApi.Persistence
{
    public class SearchRepository : ISearchRepository
    {
        SearchIndexClient _indexClient;
        SearchServiceClient _serviceClient;
        string _searchKey, _searchName;
        public IConfiguration _configuration { get; }
        // private SearchRepository _searchRepository = new SearchRepository();
        public SearchRepository(SearchServiceClient serviceClient, string searchName, string searchKey, IConfiguration configuration)
        {
            _searchKey = searchKey;
            _searchName = searchName;
            _serviceClient = serviceClient;
            _configuration = configuration;
        }
        //public SearchRepository()
        //{ }
        public void CreateIndex(string indexName)
        {
            if (_serviceClient.Indexes.ExistsAsync(indexName).Result)
            {
                _serviceClient.Indexes.DeleteAsync(indexName);
            }
            var definition = new Index()
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<publicindex>(),
               // Fields = FieldBuilder.BuildForType<System.Activator.CreateInstance("Author.Core.SearchApi.Domain", "publicindex") > (),
                Suggesters = new List<Suggester>() {new Suggester()
            {
                Name = "sg",
                SourceFields = new string[] { "Title", "RelatedCountries" }
            }}
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
        public void UploadIndexData(List<publicindex> data, string searchIndex)
        {
            _indexClient = new SearchIndexClient(_searchName, searchIndex, new SearchCredentials(_searchKey));
            var batch = IndexBatch.Upload(data);
            _indexClient.Documents.Index(batch);
        }
        public ActionResult SuggestData(bool highlights, bool fuzzy, string term, string searchIndex)
        {
          //  var response = _searchRepository.Suggest(highlights, term, fuzzy, searchIndex);






            _indexClient = new SearchIndexClient(_searchName, searchIndex, new SearchCredentials(_searchKey));
            SuggestParameters sp = new SuggestParameters()
            {
                UseFuzzyMatching = fuzzy,
                Top = 8
            };
            if (highlights)
            {
                sp.HighlightPreTag = "<b>";
                sp.HighlightPostTag = "</b>";
            }

            var response= _indexClient.Documents.Suggest(term, "sg", sp);







            List<string> suggestions = new List<string>();
            foreach (var result in response.Results)
            {
                suggestions.Add(result.Text);
            }

            // Get unique items
            List<string> uniqueItems = suggestions.Distinct().ToList();

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = uniqueItems
            };
        }
        public ActionResult AutoCompleteData(string term, string searchIndex, bool fuzzy)
        {

          //  var response = _searchRepository.AutoComplete(term, searchIndex);


            _indexClient = new SearchIndexClient(_searchName, searchIndex, new SearchCredentials(_searchKey));
            AutocompleteParameters ap = new AutocompleteParameters()
            {
                AutocompleteMode = AutocompleteMode.OneTermWithContext,
                UseFuzzyMatching = false,
                Top = 5
            };
            var response = _indexClient.Documents.Autocomplete(term, "sg", ap);



            List<string> autocomplete = response.Results.Select(x => x.Text).ToList();
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = autocomplete
            };
        }
        public async void CreateDatasource(string collectionName, string datasourceName)
        {

            string cosmosConnectString = "AccountEndpoint=" +
         _configuration["CosmosDBEndpoint"] +";AccountKey=" + _configuration["CosmosDBAccessKey"]
          + ";Database="
         + _configuration["CosmosDBName"];

            DataSource cosmosDbDataSource = DataSource.CosmosDb(
                name: datasourceName,
                cosmosDbConnectionString: cosmosConnectString,
                collectionName: collectionName,
                useChangeDetection: true);

            // The Azure Cosmos DB data source does not need to be deleted if it already exists,
            // but the connection string might need to be updated if it has changed.
            await _serviceClient.DataSources.CreateOrUpdateAsync(cosmosDbDataSource);         
        }
        public async void CreateIndexer(string indexName, string indexerName, string datasourceName)
        {

            Indexer cosmosDbIndexer = new Indexer(
          name: indexerName,
          dataSourceName: datasourceName,
          targetIndexName: indexName,
          schedule: new IndexingSchedule(System.TimeSpan.FromDays(1)));

            // Indexers keep metadata about how much they have already indexed.
            // If we already ran this sample, the indexer will remember that it already
            // indexed the sample data and not run again.
            // To avoid this, reset the indexer if it exists.
            bool exists = await _serviceClient.Indexers.ExistsAsync(cosmosDbIndexer.Name);
            if (exists)
            {
                await _serviceClient.Indexers.ResetAsync(cosmosDbIndexer.Name);
            }
            await _serviceClient.Indexers.CreateOrUpdateAsync(cosmosDbIndexer);
        }
        public async void RunIndexer(string indexerName)
        {
            await _serviceClient.Indexers.RunAsync(indexerName);
        }


        //public DocumentSuggestResult<Document> Suggest(bool highlights, string searchText, bool fuzzy, string searchIndex)
        //{
        //    _indexClient = new SearchIndexClient(_searchName, searchIndex, new SearchCredentials(_searchKey));
        //    SuggestParameters sp = new SuggestParameters()
        //    {
        //        UseFuzzyMatching = fuzzy,
        //        Top = 8
        //    };
        //    if (highlights)
        //    {
        //        sp.HighlightPreTag = "<b>";
        //        sp.HighlightPostTag = "</b>";
        //    }
        //    return _indexClient.Documents.Suggest(searchText, "sg", sp);
        //}
        //public AutocompleteResult AutoComplete(string term, string searchIndex)
        //{
        //    _indexClient = new SearchIndexClient(_searchName, searchIndex, new SearchCredentials(_searchKey));
        //    AutocompleteParameters ap = new AutocompleteParameters()
        //    {
        //        AutocompleteMode = AutocompleteMode.OneTermWithContext,
        //        UseFuzzyMatching = false,
        //        Top = 5
        //    };
        //    return _indexClient.Documents.Autocomplete(term, "sg", ap);
        //}
    }
}
