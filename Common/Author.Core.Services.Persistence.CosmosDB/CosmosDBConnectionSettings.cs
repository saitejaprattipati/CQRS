using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Core.Services.Persistence.CosmosDB
{
    public class CosmosDBConnectionSettings
    {
        public string EndpointURL { get; set; }

        public string AccessKey { get; set; }

        public string DatabaseName { get; set; }
    }
}
