using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Core.SearchApi.Domain
{
    public class publicindex
    {
        [IsFilterable, IsSearchable, IsSortable, IsFacetable]
        [System.ComponentModel.DataAnnotations.Key]
        public string Image_Id { get; set; }

        [IsFilterable, IsSearchable, IsSortable, IsFacetable]
        [Analyzer(AnalyzerName.AsString.EnLucene)]
        public string Title { get; set; }

        [IsFilterable, IsSearchable, IsSortable, IsFacetable]
        [Analyzer(AnalyzerName.AsString.EnLucene)]
        public string TeaserText { get; set; }

        [IsFilterable, IsSearchable, IsSortable, IsFacetable]
        [Analyzer(AnalyzerName.AsString.FrLucene)]
        public string RelatedCountries { get; set; }

        [IsFilterable, IsSearchable, IsSortable, IsFacetable]
        public string UpdatedDate { get; set; }
    }
}
