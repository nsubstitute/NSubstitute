namespace NSubstitute
{
    public class SubstitutionContext : ISubstitutionContext
    {
        public static ISubstitutionContext Current { get; set; }
        IInvocationHandler _lastInvocationHandler;
        ISubstituteFactory _substituteFactory;

        static SubstitutionContext()
        {
            Current = new SubstitutionContext();
        }

        SubstitutionContext()
        {
            _substituteFactory = new SubstituteFactory(this, null, null);
        }

        public SubstitutionContext(ISubstituteFactory substituteFactory)
        {
            _substituteFactory = substituteFactory;
        }

        public void LastInvocationShouldReturn<T>(T value)
        {            
            if (_lastInvocationHandler == null) throw new SubstituteException();
            _lastInvocationHandler.LastInvocationShouldReturn(value);
        }

        public void LastInvocationHandlerInvoked(IInvocationHandler _invocationHandler)
        {
            _lastInvocationHandler = _invocationHandler;
        }

        public ISubstituteFactory GetSubstituteFactory()
        {
            return _substituteFactory;
        }
    }
}