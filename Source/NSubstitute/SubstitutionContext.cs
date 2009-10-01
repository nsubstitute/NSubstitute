namespace NSubstitute
{
    public class SubstitutionContext : ISubstitutionContext
    {
        static SubstitutionContext()
        {
            Current = new SubstitutionContext();
        }

        public static ISubstitutionContext Current { get; set; }

        IInvocationHandler _lastInvocationHandler;
        ISubstituteFactory _substituteFactory;

        public SubstitutionContext()
        {
            _substituteFactory = new SubstituteFactory(this, null, null);
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