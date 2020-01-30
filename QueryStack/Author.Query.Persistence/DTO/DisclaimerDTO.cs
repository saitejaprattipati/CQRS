using Author.Core.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Query.Persistence.DTO
{
    public class DisclaimerResult
    {
        public DisclaimerResult()
        {
            Disclaimers = new List<DisclaimerDTO>();
        }
        public List<DisclaimerDTO> Disclaimers { get; set; }
    }

    public class DisclaimerDTO
    {
        public int? DisclaimerId { get; set; }

        [JsonProperty("groupname")]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string Name { get; set; }

        [JsonProperty("providername")]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string ProviderName { get; set; }

        [JsonProperty("providerterms")]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string ProviderTerms { get; set; }

        [JsonProperty("homecountry")]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string HomeCountry { get; set; }

        public int? LanguageId { get; set; }
    }
}
