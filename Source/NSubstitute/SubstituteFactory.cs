namespace NSubstitute
{
    public class SubstituteFactory : ISubstituteFactory
    {
        readonly ISubstitutionContext _context;
        readonly IInvocationHandlerFactory _invocationHandlerFactory;
        readonly ISubstituteBuilder _substituteBuilder;
        
        public SubstituteFactory(ISubstitutionContext context, 
                                    IInvocationHandlerFactory invocationHandlerFactory, 
                                    ISubstituteBuilder substituteBuilder)
        {
            _context = context;
            _invocationHandlerFactory = invocationHandlerFactory;
            _substituteBuilder = substituteBuilder;
        }

        public T Create<T>()
        {
            var invocationHandler = _invocationHandlerFactory.CreateInvocationHandler(_context);
            var interceptor = _substituteBuilder.CreateInterceptor(invocationHandler);
            return _substituteBuilder.GenerateProxy<T>(interceptor);
        }
    }
}