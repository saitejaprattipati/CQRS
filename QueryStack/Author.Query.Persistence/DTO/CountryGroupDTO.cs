using Author.Core.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Query.Persistence.DTO
{
    public class CountryGroupResult 
    {
        public CountryGroupResult()
        {
            CountryGroups = new List<CountryGroupDTO>();
        }

        [JsonProperty(PropertyName = "countrygroupcontent")]
        public List<CountryGroupDTO> CountryGroups { get; set; }
    }

    public class CountryGroupDTO
    {
        public CountryGroupDTO()
        {
            CountryGroupAssociatedCountries = new List<CountryDTO>();
        }

        public int? CountryGroupId { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string GroupName { get; set; } = "";
        public List<CountryDTO> CountryGroupAssociatedCountries { get; set; }
    }
}
