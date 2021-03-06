using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class UserDevices
    {
        public int UserDeviceId { get; set; }
        public int WebsiteUserId { get; set; }
        public string Platform { get; set; }
        public string DeviceIdentifier { get; set; }
        public string InstallationId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual WebsiteUsers WebsiteUser { get; set; }
    }
}
