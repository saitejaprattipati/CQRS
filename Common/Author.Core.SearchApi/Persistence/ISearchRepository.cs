using Author.Core.SearchApi.Domain;
using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Author.Core.SearchApi.Persistence
{
    public interface ISearchRepository
    {
        public void CreateIndex(string indexName);
        public void DeleteIndex(string indexName);
        public void UploadIndexData(List<publicindex> data, string searchIndex);
        public ActionResult SuggestData(bool highlights, bool fuzzy, string term, string searchIndex);
        public ActionResult AutoCompleteData(string term, string searchIndex,bool fuzzy);
        public void CreateDatasource(string collectionName, string datasourceName);
        public void CreateIndexer(string indexName, string indexerName, string datasourceName);
        public void RunIndexer(string indexerName);
    }
}
