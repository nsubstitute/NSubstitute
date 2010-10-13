using System.Collections.Generic;

namespace NSubstitute.Core
{
    public class EventHandlerRegistry : IEventHandlerRegistry
    {
        readonly Dictionary<string, List<object>> _handlersForEvent = new Dictionary<string, List<object>>();

        public void Add(string eventName, object handler)
        {
            Handlers(eventName).Add(handler);    
        }

        public void Remove(string eventName, object handler)
        {
            Handlers(eventName).Remove(handler);
        }

        public IEnumerable<object> GetHandlers(string eventName)
        {
            var snapshotOfHandlersForEvent = Handlers(eventName).ToArray();
            return snapshotOfHandlersForEvent;
        }

        private List<object> Handlers(string eventName)
        {
            if (!_handlersForEvent.ContainsKey(eventName))
                _handlersForEvent[eventName] = new List<object>();
            return _handlersForEvent[eventName];
        }
    }
}