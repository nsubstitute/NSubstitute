namespace NSubstitute
{
    public class SubstitutionContext : ISubstitutionContext
    {
        static SubstitutionContext()
        {
            Current = new SubstitutionContext();
        }

        private IInvocationHandler _lastInvocationHandler;

        public static void SetCurrent(ISubstitutionContext context)
        {
            Current = context;
        }

        public static ISubstitutionContext Current { get; private set; }
        
        public void LastInvocationShouldReturn<T>(T value)
        {            
            if (_lastInvocationHandler == null) throw new SubstitutionException();
            _lastInvocationHandler.LastInvocationShouldReturn(value);
        }

        public void LastInvocationHandlerInvoked(IInvocationHandler _invocationHandler)
        {
            _lastInvocationHandler = _invocationHandler;
        }

    }
}