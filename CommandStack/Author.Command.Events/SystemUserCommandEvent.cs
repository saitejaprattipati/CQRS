using Author.Core.Services.EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Command.Events
{
    public class SystemUserCommandEvent:IntegrationEvent
    {
        [JsonProperty("SystemUserId")]
        public int SystemUserId { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Level")]
        public string Level { get; set; }

        [JsonProperty("WorkPhoneNumber")]
        public string WorkPhoneNumber { get; set; }

        [JsonProperty("MobilePhoneNumber")]
        public string MobilePhoneNumber { get; set; }

        [JsonProperty("Location")]
        public string Location { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Role")]
        public int Role { get; set; }

        [JsonProperty("TimeZone")]
        public TimeSpan? TimeZone { get; set; }

        [JsonProperty("CreatedBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("UpdatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("UpdatedDate")]
        public DateTime UpdatedDate { get; set; }

        [JsonProperty("CountryId")]
        public int CountryId { get; set; }

        [JsonProperty("IsPrimary")]
        public bool IsPrimary { get; set; }

        [JsonProperty("SystemUserAssociatedCountryId")]
        public int SystemUserAssociatedCountryId { get; set; }

    }
}
