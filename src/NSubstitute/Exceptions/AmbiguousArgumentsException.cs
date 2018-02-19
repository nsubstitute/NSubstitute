using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core.Arguments;
using static System.Environment;

namespace NSubstitute.Exceptions
{
    public class AmbiguousArgumentsException : SubstituteException
    {
        public AmbiguousArgumentsException(IEnumerable<IArgumentSpecification> queuedSpecifications)
            : this(BuildExceptionMessage(queuedSpecifications))
        {
        }

        public AmbiguousArgumentsException(string message) : base(message)
        {
        }

        private static string BuildExceptionMessage(IEnumerable<IArgumentSpecification> queuedSpecifications)
        {
            return
                $"Cannot determine argument specifications to use. Please use specifications for all arguments of the same type.{NewLine}" +
                $"All queued specifications:{NewLine}" +
                $"{FormatSpecifications(queuedSpecifications)}{NewLine}";
        }

        private static string FormatSpecifications(IEnumerable<IArgumentSpecification> specifications)
        {
            return string.Join(NewLine, specifications.Select(spec => "    " + spec.ToString()));
        }
    }
}
