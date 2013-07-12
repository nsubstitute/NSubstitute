using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class CallBaseSpecifications : ICallBaseSpecifications
    {
        readonly List<ICallSpecification> _callSpecifications = new List<ICallSpecification>();

        public void Add(ICallSpecification callSpecification)
        {
            _callSpecifications.Add(callSpecification);
        }

        public void Remove(ICallSpecification callSpecification)
        {
            _callSpecifications.RemoveAll(cs => cs.IsEqualsTo(callSpecification));
        }

        public bool DoesCallBase(ICall callInfo)
        {
            return _callSpecifications.Any(cs => cs.IsSatisfiedBy(callInfo));
        }
    }
}