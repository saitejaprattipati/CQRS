using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class TokenCacheEntries
    {
        public int TokenCacheEntryId { get; set; }
        public string UserObjectId { get; set; }
        public byte[] CacheBits { get; set; }
        public DateTime LastWrite { get; set; }
    }
}
