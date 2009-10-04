namespace NSubstitute
{
    public class SubstituteFactory : ISubstituteFactory
    {
        readonly ISubstitutionContext _context;
        readonly IInvocationHandlerFactory _invocationHandlerFactory;
        readonly IProxyFactory _proxyFactory;
        
        public SubstituteFactory(ISubstitutionContext context, 
                                    IInvocationHandlerFactory invocationHandlerFactory, 
                                    IProxyFactory proxyFactory)
        {
            _context = context;
            _invocationHandlerFactory = invocationHandlerFactory;
            _proxyFactory = proxyFactory;
        }

        public T Create<T>()
        {
            var invocationHandler = _invocationHandlerFactory.CreateInvocationHandler(_context);
            return _proxyFactory.GenerateProxy<T>(invocationHandler);
        }
    }
}