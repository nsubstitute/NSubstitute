using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface IEventHandlerRegistry
    {
        void Add(string eventName, object handler);
        void Remove(string eventName, object handler);
        IEnumerable<object> GetHandlers(string eventName);
    }
}