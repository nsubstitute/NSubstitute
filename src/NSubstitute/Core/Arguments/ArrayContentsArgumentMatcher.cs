using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class ArrayContentsArgumentMatcher : IArgumentMatcher, IArgumentFormatter
    {
        private readonly IArgumentSpecification[] _argumentSpecifications;
        private readonly bool _isParams;

        public ArrayContentsArgumentMatcher(IEnumerable<IArgumentSpecification> argumentSpecifications, bool isParams)
        {
            _argumentSpecifications = argumentSpecifications.ToArray();
            _isParams = isParams;
        }

        public bool IsSatisfiedBy(object argument)
        {
            if (argument != null)
            {
                var argumentArray = ((IEnumerable) argument).Cast<object>().ToArray();
                if (argumentArray.Length == _argumentSpecifications.Count())
                {
                    return
                        _argumentSpecifications.Select(
                            (value, index) => value.IsSatisfiedBy(argumentArray[index])).All(x => x);
                }
            }
            return false;
        }

        public override string ToString()
        {
            string output = string.Join(", ", _argumentSpecifications.Select(x => x.ToString()));
            return _isParams ? output : $"[{output}]";
        }

        public string Format(object argument, bool highlight)
        {
            var argArray = (argument as IEnumerable)?.Cast<object>().ToArray() ?? new object[0];
            string output = Format(argArray, _argumentSpecifications).Join(", ");

            return _isParams ? output : $"[{output}]";
        }

        private IEnumerable<string> Format(object[] args, IArgumentSpecification[] specs)
        {
            if (specs.Any() && !args.Any())
            {
                return new [] { "**" };
            }
            return args.Select((arg, index) => {
                var hasSpecForThisArg = index < specs.Length;
                return hasSpecForThisArg ? specs[index].FormatArgument(arg) : ArgumentFormatter.Default.Format(arg, true);
            });
        }
    }
}