using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public abstract class ArgumentParamsExtractor : IArgumentParamsExtractor
    {
        public IEnumerable<object> GetWithExtractedArguments(IEnumerable<object> arguments)
        {
            if (arguments.Any())
            {
                List<object> extractedArguments = arguments.ToList();

                extractedArguments.RemoveAt(GetParamsArgumentIndex(arguments));
                extractedArguments.AddRange(GetExtractedParamsArguments(arguments));

                return extractedArguments;
            }

            throw new ArgumentException("No arguments given.", "arguments");
        }

        public IEnumerable<int> GetWithExtractedArgumentsToHighlight(IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            if (ParamsArgumentIsHighlighted(arguments, argumentsToHighlight))
            {
                List<int> extractedArgumentsToHighlight = argumentsToHighlight.ToList();

                extractedArgumentsToHighlight.Remove(GetParamsArgumentIndex(arguments));
                extractedArgumentsToHighlight.AddRange(GetExtractedParamsArgumentIndeces(arguments));

                return extractedArgumentsToHighlight;
            }

            return argumentsToHighlight;
        }

        protected abstract IEnumerable<object> GetExtractedParamsArguments(IEnumerable<object> arguments);

        private IEnumerable<int> GetExtractedParamsArgumentIndeces(IEnumerable<object> arguments)
        {
            IEnumerable<object> extractedParamsArguments = GetExtractedParamsArguments(arguments);

            IEnumerable<int> extractedParamsArgumentIndeces = extractedParamsArguments.Select((arg, index) => GetParamsArgumentIndex(arguments) + index);

            return extractedParamsArgumentIndeces;
        }

        private bool ParamsArgumentIsHighlighted(IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            return argumentsToHighlight.Any() && argumentsToHighlight.Contains(GetParamsArgumentIndex(arguments));
        }

        private int GetParamsArgumentIndex(IEnumerable<object> arguments)
        {
            //Params argument is always the last argument.
            return arguments.Count() - 1;
        }
    }
}