using System;

namespace NSubstitute.Core
{
    public class SubstituteState
    {
        public SubstituteState(ISubstitutionContext substitutionContext)
        {
            CallInfoFactory = new CallInfoFactory();
            CallStack = new CallStack();
            CallResults = new CallResults(CallInfoFactory);
            CallActions = new CallActions();
            PropertyHelper = new PropertyHelper();
            CallSpecificationFactory = new CallSpecificationFactory(substitutionContext);
            ResultSetter = new ResultSetter(CallStack, CallResults, CallSpecificationFactory);
            EventHandlerRegistry = new EventHandlerRegistry();
            CallNotReceivedExceptionThrower = new CallNotReceivedExceptionThrower();
        }

        public CallStack CallStack { get; private set; }
        public ICallResults CallResults { get; private set; }
        public IPropertyHelper PropertyHelper { get; private set; }
        public ICallSpecificationFactory CallSpecificationFactory { get; private set; }
        public IResultSetter ResultSetter { get; private set; }
        public IEventHandlerRegistry EventHandlerRegistry { get; private set; }
        public ICallActions CallActions { get; private set; }
        public CallInfoFactory CallInfoFactory { get; private set; }
        public ICallNotReceivedExceptionThrower CallNotReceivedExceptionThrower { get; private set; }
    }
}