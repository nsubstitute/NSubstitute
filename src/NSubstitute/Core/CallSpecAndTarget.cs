namespace NSubstitute.Core
{
    public class CallSpecAndTarget
    {
        public ICallSpecification CallSpecification { get; }
        public object Target { get; }

        public CallSpecAndTarget(ICallSpecification callSpecification, object target)
        {
            CallSpecification = callSpecification;
            Target = target;
        }
    }
}