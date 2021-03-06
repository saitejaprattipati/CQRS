using System;
using System.Collections.Generic;
using System.Text;
namespace Author.Core.Services.EventBus.Interfaces
{
    public interface IEventBus
    {
        void Subscribe<T, TH>()
          where T : IntegrationEvent
          where TH : IIntegrationEventHandler<T>;
        void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;

        void Publish(IntegrationEvent @event);
    }
}
