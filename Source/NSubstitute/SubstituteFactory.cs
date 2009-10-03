namespace NSubstitute
{
    public class SubstituteFactory : ISubstituteFactory
    {
        readonly ISubstitutionContext _context;
        readonly IInvocationHandlerFactory _invocationHandlerFactory;
        readonly IProxyGenerator _proxyGenerator;
        
        public SubstituteFactory(ISubstitutionContext context, 
                                    IInvocationHandlerFactory invocationHandlerFactory, 
                                    IProxyGenerator proxyGenerator)
        {
            _context = context;
            _invocationHandlerFactory = invocationHandlerFactory;
            _proxyGenerator = proxyGenerator;
        }

        public T Create<T>()
        {
            var invocationHandler = _invocationHandlerFactory.CreateInvocationHandler(_context);
            return _proxyGenerator.GenerateProxy<T>(invocationHandler);
        }
    }
}