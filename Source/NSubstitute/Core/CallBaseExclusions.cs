using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class CallBaseExclusions : ICallBaseExclusions
    {
        readonly List<ICallSpecification> _excludedCallSpecs = new List<ICallSpecification>();

        public void Exclude(ICallSpecification callSpecification)
        {
            _excludedCallSpecs.Add(callSpecification);
        }

        public bool IsExcluded(ICall callInfo)
        {
            return _excludedCallSpecs.Any(cs => cs.IsSatisfiedBy(callInfo));
        }
    }
}