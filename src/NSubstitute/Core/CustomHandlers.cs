using System.Collections.Generic;

namespace NSubstitute.Core
{
    public class CustomHandlers : ICustomHandlers
    {
        private readonly List<ICallHandler> _handlers = new();
        private readonly ISubstituteState _substituteState;

        public IReadOnlyCollection<ICallHandler> Handlers => _handlers;

        public CustomHandlers(ISubstituteState substituteState)
        {
            _substituteState = substituteState;
        }

        public void AddCustomHandlerFactory(CallHandlerFactory factory)
        {
            _handlers.Add(factory.Invoke(_substituteState));
        }
    }
}