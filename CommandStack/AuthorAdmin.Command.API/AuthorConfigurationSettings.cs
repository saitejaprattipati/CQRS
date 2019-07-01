using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorAdmin.Command.API
{
    public class AuthorConfigurationSettings
    {
        public string DBConnectionString { get; set; }

        public string ServiceBusConnection { get; set; }

        public string ServiceBusUserName { get; set; }

        public string ServiceBusUserPassword { get; set; }
    }
}
