namespace NSubstitute.Core
{
    public class CallSpecAndTarget
    {
        public ICallSpecification CallSpecification { get; private set; }
        public object Target { get; private set; }

        public CallSpecAndTarget(ICallSpecification callSpecification, object target)
        {
            CallSpecification = callSpecification;
            Target = target;
        }
    }
}