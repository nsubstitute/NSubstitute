using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentsFormatter : IArgumentsFormatter
    {
        private IArgumentFormatter _argumentFormatter;

        public ArgumentsFormatter(IArgumentFormatter argumentFormatter)
        {
            _argumentFormatter = argumentFormatter;
        }

        public string Format(IEnumerable<IArgumentFormatInfo> argumentFormatInfos)
        {
            return string.Join(", ", argumentFormatInfos.Select(arg => arg.Format(_argumentFormatter)).ToArray());
        }
    }
}