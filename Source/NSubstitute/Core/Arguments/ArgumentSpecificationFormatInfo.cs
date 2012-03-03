namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecificationFormatInfo : IArgumentFormatInfo
    {
        private IArgumentSpecification _argumentSpecification;
        private bool _isHighlighted;

        public ArgumentSpecificationFormatInfo(IArgumentSpecification argumentSpecification, bool isHighlighted)
        {
            _argumentSpecification = argumentSpecification;
            _isHighlighted = isHighlighted;
        }

        public string Format(IArgumentFormatter argumentFormatter)
        {
            var argAsString = argumentFormatter.Format(_argumentSpecification);
            return _isHighlighted ? "*" + argAsString + "*" : argAsString;
        }
    }
}