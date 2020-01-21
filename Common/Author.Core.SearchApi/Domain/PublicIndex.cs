using System;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Newtonsoft.Json;

namespace Author.Core.SearchApi.Domain
{
    public class PublicIndex
    {
        [IsFilterable, IsSearchable, IsSortable, IsFacetable]
        [System.ComponentModel.DataAnnotations.Key]        
        public string ImageId { get; set; }
      
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
