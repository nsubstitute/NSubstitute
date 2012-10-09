namespace NSubstitute.Core
{
    public class Query
    {
        public virtual void Add(object target, ICallSpecification callSpecification)
        {
        }

        public virtual bool IsRunning { get; private set; }
    }
}