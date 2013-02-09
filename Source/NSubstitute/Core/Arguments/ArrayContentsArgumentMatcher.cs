﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class ArrayContentsArgumentMatcher : IArgumentMatcher, IArgumentFormatter
    {
        private static readonly IArgumentFormatter DefaultArgumentFormatter = new ArgumentFormatter();
        private readonly IEnumerable<IArgumentSpecification> _argumentSpecifications;

        public ArrayContentsArgumentMatcher(IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            _argumentSpecifications = argumentSpecifications;
        }

        public bool IsSatisfiedBy(object argument)
        {
            if (argument != null)
            {
                var argumentArray = ((IEnumerable) argument).Cast<object>();
                if (argumentArray.Count() == _argumentSpecifications.Count())
                {
                    return
                        _argumentSpecifications.Select(
                            (value, index) => value.IsSatisfiedBy(argumentArray.ElementAt(index))).All(x => x);
                }
            }
            return false;
        }

        public override string ToString()
        {
            return string.Join(", ", _argumentSpecifications.Select(x => x.ToString()).ToArray());
        }

        public string Format(object argument, bool highlight)
        {
            var specsArray = _argumentSpecifications.ToArray();
            var argArray = (argument as object[]) ?? new object[0];
            return Format(argArray, specsArray).Join(", ");
        }

        private IEnumerable<string> Format(object[] args, IArgumentSpecification[] specs)
        {
            return args.Select((arg, index) => {
                var hasSpecForThisArg = index < specs.Length;
                return hasSpecForThisArg ? specs[index].FormatArgument(arg) : DefaultArgumentFormatter.Format(arg, true);
            });
        }
    }
}