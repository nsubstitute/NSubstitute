namespace NSubstitute
{
    public class SubstituteState
    {
        public SubstituteState()
        {
            CallStack = new CallStack();
            CallResults = new CallResults();
            ReflectionHelper = new ReflectionHelper();
            CallSpecificationFactory = new CallSpecificationFactory(SubstitutionContext.Current);
            ResultSetter = new ResultSetter(CallStack, CallResults, CallSpecificationFactory);
        }

        public ICallStack CallStack { get; private set; }
        public ICallResults CallResults { get; private set; }
        public IReflectionHelper ReflectionHelper { get; private set; }
        public ICallSpecificationFactory CallSpecificationFactory { get; private set; }
        public IResultSetter ResultSetter { get; private set; }
    }
}