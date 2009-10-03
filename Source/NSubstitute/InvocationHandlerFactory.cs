namespace NSubstitute
{
    public class InvocationHandlerFactory : IInvocationHandlerFactory
    {
        public IInvocationHandler CreateInvocationHandler(ISubstitutionContext substitutionContext)
        {
            return new InvocationHandler(new InvocationStack(), new InvocationResults(), substitutionContext);
        }
    }
}