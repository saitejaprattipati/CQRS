using System;
using System.Collections.Generic;

namespace Author.Core.Services.EventBus
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; }
        public DateTime CreationDate { get; }
        public string EventType { get; set; }

    }
}
