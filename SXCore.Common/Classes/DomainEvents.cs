using SXCore.Common.Contracts;
using SXCore.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Classes
{
    public static class DomainEvents
    {
        //so that each thread has its own callbacks
        [ThreadStatic] 
        private static List<Delegate> actions;

        // Container with Dependency Injections
        public static IDependencyResolver Container { get; set; }

        // Registers a callback for the given domain event
        public static void Register<T>(Action<T> callback) where T:IDomainEvent
        {
            if (actions == null)
                actions = new List<Delegate>();

            actions.Add(callback);
        }

        // Clears callbacks passed to Register on the current thread
        public static void ClearCallbacks()
        {
            actions = null;
        }

        // Raises the given domain event
        public static void Raise<T>(T args) where T:IDomainEvent
        {
            if (Container != null)
            {
                var handlers = Container.ResolveAll<IDomainEventHandler<T>>().ToArray();
                foreach (var handler in handlers)
                    handler.Handle(args);
            }

            if (actions != null)
            {
                foreach (var action in actions)
                    if (action is Action<T>)
                        ((Action<T>)action)(args);
            }
        }
    }
}
