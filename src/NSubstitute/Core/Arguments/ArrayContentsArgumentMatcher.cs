﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class ArrayContentsArgumentMatcher : IArgumentMatcher, IArgumentFormatter
    {
        private readonly IArgumentSpecification[] _argumentSpecifications;

        public ArrayContentsArgumentMatcher(IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            _argumentSpecifications = argumentSpecifications.ToArray();
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
            return string.Join(", ", _argumentSpecifications.Select(x => x.ToString()));
        }

        public string Format(object argument, bool highlight)
        {
            var enumerableArgs = argument as IEnumerable;
            var argArray = enumerableArgs != null ? enumerableArgs.Cast<object>().ToArray() : new object[0];
            return Format(argArray, _argumentSpecifications).Join(", ");
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