namespace NSubstitute
{
    public class SubstituteFactory : ISubstituteFactory
    {
        readonly ISubstitutionContext _context;
        readonly ICallHandlerFactory _callHandlerFactory;
        readonly IProxyFactory _proxyFactory;
        
        public SubstituteFactory(ISubstitutionContext context, 
                                    ICallHandlerFactory _callHandlerFactory, 
                                    IProxyFactory proxyFactory)
        {
            _context = context;
            this._callHandlerFactory = _callHandlerFactory;
            _proxyFactory = proxyFactory;
        }

        public T Create<T>() where T : class
        {
            var callHandler = _callHandlerFactory.CreateCallHandler(_context);
            return _proxyFactory.GenerateProxy<T>(callHandler);
        }
    }
}