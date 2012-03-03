namespace NSubstitute.Core.Arguments
{
    public class ObjectArgumentFormatInfo : IArgumentFormatInfo
    {
        private object _argument;
        private bool _isHighlighted;

        public ObjectArgumentFormatInfo(object argument, bool isHighlighted)
        {
            _argument = argument;
            _isHighlighted = isHighlighted;
        }

        public string Format(IArgumentFormatter argumentFormatter)
        {
            var argAsString = argumentFormatter.Format(_argument);
            return _isHighlighted ? "*" + argAsString + "*" : argAsString;

        }
    }
}