using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class ObjectParamsArrayArgumentFormatInfo : IArgumentFormatInfo
    {
        private object[] _arguments;
        private bool _isHighlighted;

        public ObjectParamsArrayArgumentFormatInfo(object[] arguments, bool isHighlighted)
        {
            _arguments = arguments;
            _isHighlighted = isHighlighted;
        }

        public string Format(IArgumentFormatter argumentFormatter)
        {
            var argAsString = string.Join(", ", _arguments.Select(arg => argumentFormatter.Format(arg)).ToArray());
            return _isHighlighted ? "*" + argAsString + "*" : argAsString;
        }
    }
}