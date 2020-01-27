using Author.Core.SearchApi.Domain;
using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Core.SearchApi.Persistence
{
   public interface ISearchRepository
    {
        public void CreateIndex(string indexName);
        public void DeleteIndex(string indexName);
        public void UploadIndexData(List<publicindex> data, string searchIndex);
    }
}
