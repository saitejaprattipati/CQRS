using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Author.Core.Services.EventBus.Interfaces
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
