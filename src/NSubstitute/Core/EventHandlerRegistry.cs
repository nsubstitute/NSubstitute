using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public class EventHandlerRegistry : IEventHandlerRegistry
    {
        // Collection consideration - events are used very rarely, so it makes no sense to allocate concurrent collection.
        // Events are not expected to be configured/raised concurrently, so simple locking should be sufficient.
        // List lookup is O(n), but for really small size performance is comparable to dictionary.
        // Given that normally a few events are configured only, it should be totally fine.
        private readonly List<Tuple<string, List<object>>> _handlersForEvent = new();

        public void Add(string eventName, object handler)
        {
            lock (_handlersForEvent)
            {
                Handlers(eventName).Add(handler);
            }
        }

        public void Remove(string eventName, object handler)
        {
            lock (_handlersForEvent)
            {
                Handlers(eventName).Remove(handler);
            }
        }

        public IEnumerable<object> GetHandlers(string eventName)
        {
            lock (_handlersForEvent)
            {
                // Make snapshot to make code thread-safe.
                return Handlers(eventName).ToArray();
            }
        }

        private List<object> Handlers(string eventName)
        {
            foreach (var pair in _handlersForEvent)
            {
                if (pair.Item1 == eventName)
                {
                    return pair.Item2;
                }
            }

            var newPair = Tuple.Create(eventName, new List<object>());
            _handlersForEvent.Add(newPair);
            return newPair.Item2;
        }
    }
}