using System;
using System.Collections.Generic;

namespace Author.Core.Services.EventBus
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public Guid id { get; set; }
        public DateTime CreationDate { get; }
        public string EventType { get; set; }
        public string Discriminator { get; set; }

    }
}
