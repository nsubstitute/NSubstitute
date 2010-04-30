using System;

namespace NSubstitute
{
    public class SubstituteState
    {
        public SubstituteState(ISubstitutionContext substitutionContext)
        {
            CallStack = new CallStack();
            CallResults = new CallResults();
            CallActions = new CallActions();
            ReflectionHelper = new ReflectionHelper();
            CallSpecificationFactory = new CallSpecificationFactory(substitutionContext);
            ResultSetter = new ResultSetter(CallStack, CallResults, CallSpecificationFactory);
            EventHandlerRegistry = new EventHandlerRegistry();
        }

        public ICallStack CallStack { get; private set; }
        public ICallResults CallResults { get; private set; }
        public IReflectionHelper ReflectionHelper { get; private set; }
        public ICallSpecificationFactory CallSpecificationFactory { get; private set; }
        public IResultSetter ResultSetter { get; private set; }
        public IEventHandlerRegistry EventHandlerRegistry { get; private set; }
        public ICallActions CallActions { get; private set; }
    }
}