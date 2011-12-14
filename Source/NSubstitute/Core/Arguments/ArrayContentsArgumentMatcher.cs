using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class ArrayContentsArgumentMatcher : IArgumentMatcher
    {
        private readonly IEnumerable<IArgumentSpecification> _argumentSpecifications;

        public ArrayContentsArgumentMatcher(IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            _argumentSpecifications = argumentSpecifications;
        }

        public bool IsSatisfiedBy(object argument)
        {
            if (argument != null)
            {
                var argumentArray = (IEnumerable<object>)argument;
                if (argumentArray.Count() == _argumentSpecifications.Count())
                {
                    return _argumentSpecifications.Select((value, index) => value.IsSatisfiedBy(argumentArray.ElementAt(index))).All(x => x);
                }
            }
            return false;
        }

        public override string ToString()
        {
            return string.Join(", ", _argumentSpecifications.Select(x => x.ToString()).ToArray());
        }
    }
}