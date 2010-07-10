using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class ArgumentsFormatter : IArgumentsFormatter
    {
        private readonly IArgumentFormatter _argumentFormatter;

        public ArgumentsFormatter(IArgumentFormatter argumentFormatter)
        {
            _argumentFormatter = argumentFormatter;
        }

        public string Format(IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            return string.Join(", ", arguments.Select((argument, index) => FormatArg(argument, argumentsToHighlight.Contains(index))).ToArray());
        }

        private string FormatArg(object argument, bool highlight)
        {
            var argAsString = _argumentFormatter.Format(argument);
            return highlight ? "*" + argAsString + "*" : argAsString;
        }
    }
}