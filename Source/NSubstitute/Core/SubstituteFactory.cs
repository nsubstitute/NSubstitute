namespace NSubstitute.Core
{
    public class SubstituteFactory : ISubstituteFactory
    {
        readonly ISubstitutionContext _context;
        readonly ICallRouterFactory _callRouterFactory;
        readonly IProxyFactory _proxyFactory;
        
        public SubstituteFactory(ISubstitutionContext context, 
                                    ICallRouterFactory callRouterFactory, 
                                    IProxyFactory proxyFactory)
        {
            _context = context;
            _callRouterFactory = callRouterFactory;
            _proxyFactory = proxyFactory;
        }

        public T Create<T>() where T : class
        {
            var callRouter = _callRouterFactory.Create(_context);
            return _proxyFactory.GenerateProxy<T>(callRouter);
        }
    }
}