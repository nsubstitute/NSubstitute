using System.Collections.Generic;

namespace NSubstitute
{
    public interface IEventHandlerRegistry
    {
        void Add(string eventName, object handler);
        void Remove(string eventName, object handler);
        IEnumerable<object> GetHandlers(string eventName);
    }
}