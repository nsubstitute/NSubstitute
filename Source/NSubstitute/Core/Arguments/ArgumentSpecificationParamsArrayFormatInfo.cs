using System;
using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecificationParamsArrayFormatInfo : IArgumentFormatInfo
    {
        private IArgumentSpecification _argumentSpecification;
        private bool _isHighlighted;

        public ArgumentSpecificationParamsArrayFormatInfo(IArgumentSpecification argumentSpecification, bool isHighlighted)
        {
            _argumentSpecification = argumentSpecification;
            _isHighlighted = isHighlighted;
        }

        public string Format(IArgumentFormatter argumentFormatter)
        {
            var arrayContentsArgumentMatcher = _argumentSpecification.ArgumentMatcher as ArrayContentsArgumentMatcher;

            if (arrayContentsArgumentMatcher == null)
            {
                throw new ArgumentException("Last IArgumentSpecification does not contain an ArrayContentsArgumentMatcher.");
            }

            var argAsString = string.Join(", ", arrayContentsArgumentMatcher.ArgumentSpecifications.Select(arg => argumentFormatter.Format(arg)).ToArray());
            return _isHighlighted ? "*" + argAsString + "*" : argAsString;
        }
    }
}
