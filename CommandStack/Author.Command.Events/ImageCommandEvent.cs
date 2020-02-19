using Author.Core.Services.EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Command.Events
{
    public class ImageCommandEvent : IntegrationEvent
    {
        [JsonProperty("ImageId")]
        public int ImageId { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ImageType")]
        public int ImageType { get; set; }

        [JsonProperty("CountryId")]
        public int? CountryId { get; set; }

        [JsonProperty("Keyword")]
        public string Keyword { get; set; }

        [JsonProperty("Source")]
        public string Source { get; set; }

        [JsonProperty("Copyright")]
        public string Copyright { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("FilePath")]
        public string FilePath { get; set; }

        [JsonProperty("FileType")]
        public string FileType { get; set; }

        [JsonProperty("CreatedBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("UpdatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("UpdatedDate")]
        public DateTime UpdatedDate { get; set; }

        [JsonProperty("Edited")]
        public bool IsEdited { get; set; }

        [JsonProperty("EmpGUID")]
        public string EmpGuid { get; set; }

        [JsonProperty("PartitionKey")]
        public string PartitionKey { get; set; }
    }
}
