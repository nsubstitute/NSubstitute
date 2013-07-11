using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class CallBaseSpecifications : ICallBaseSpecifications
    {
        readonly List<ICallSpecification> _callSpecifications = new List<ICallSpecification>();

        public void Add(ICallSpecification callSpec)
        {
            _callSpecifications.Add(callSpec);
        }

        public bool DoesCallBase(ICall callInfo)
        {
            return _callSpecifications.Any(cs => cs.IsSatisfiedBy(callInfo));
        }
    }
}