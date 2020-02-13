using Author.Core.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Query.Persistence.DTO
{
    public class TaxTagGroupsResult
    {
        public TaxTagGroupsResult()
        {
            TaxTagGroups = new List<TaxTagGroupDTO>();
        }
        public List<TaxTagGroupDTO> TaxTagGroups { get; set; }
    }

    public class TaxTagsResult
    {
        public TaxTagsResult()
        {
            TaxTags = new List<TaxTagsDTO>();
        }
        public List<TaxTagsDTO> TaxTags { get; set; }
    }

    public class TaxTagGroupDTO
    {
        public int? TaxTagId { get; set; }

        public int? ParentTagId { get; set; }
        
        [JsonProperty("displayname")]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string DisplayName { get; set; }

        public int? LanguageId { get; set; }

        public List<TaxTagsDTO> AssociatedTags { get; set; }
    }

    public class TaxTagsDTO
    {
        public int? TaxTagId { get; set; }

        public int? ParentTagId { get; set; }

        public int? CountryId { get; set; }

        [JsonProperty("displayname")]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string DisplayName { get; set; }

        public int? LanguageId { get; set; }

        public List<CountryDTO> RelatedCountries { get; set; }
    }
}
