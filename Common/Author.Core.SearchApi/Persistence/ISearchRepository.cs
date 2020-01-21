using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Core.SearchApi.Persistence
{
    interface ISearchRepository
    {
        public void CreateIndex(string indexName, SearchServiceClient serviceClient);
    }
}
