using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Interfaces
{
    /// <summary>
    /// Defines Domain Event to be fired
    /// </summary>
    public interface IDomainEvent
    {
    }

    /// <summary>
    /// Defines the method that can handle Domain Event of type T
    /// </summary>
    /// <typeparam name="T">Domain Event type of IEvent</typeparam>
    public interface IDomainEventHandler<T> where T : IDomainEvent
    {
        void Handle(T args);
    }

    public interface IDomainEventHolder
    {
        IEnumerable<IDomainEvent> Events { get; }
        void Register<TEvent>(TEvent eventToHold) where TEvent : IDomainEvent;
        void Clear();
    }
}
