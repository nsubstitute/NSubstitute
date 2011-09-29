using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class ArrayContentsArgumentSpecification : IArgumentSpecification
    {
        private readonly Type _forType;
        private readonly IEnumerable<IArgumentSpecification> _argumentSpecifications;

        public ArrayContentsArgumentSpecification(IEnumerable<IArgumentSpecification> argumentSpecifications, Type forType)
        {
            _argumentSpecifications = argumentSpecifications;
            _forType = forType;
            Action = x => { };
        }

        public Type ForType { get { return _forType; } }
        public Action<object> Action { get; set; }

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