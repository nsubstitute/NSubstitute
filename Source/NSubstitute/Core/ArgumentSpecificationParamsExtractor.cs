using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class ArgumentSpecificationParamsExtractor : ArgumentParamsExtractor
    {
        protected override IEnumerable<object> GetExtractedParamsArguments(IEnumerable<object> arguments)
        {
            IArgumentSpecification paramsArgument = arguments.Last() as IArgumentSpecification;

            if (paramsArgument == null)
            {
                throw new ArgumentException("Last argument is not an IArgumentSpecification.", "arguments");
            }

            ArrayContentsArgumentMatcher arrayContentsArgumentMatcher =
                paramsArgument.ArgumentMatcher as ArrayContentsArgumentMatcher;

            if(arrayContentsArgumentMatcher == null)
            {
                throw new ArgumentException("Last IArgumentSpecification does not contain an ArrayContentsArgumentMatcher.");
            }

            return arrayContentsArgumentMatcher.ArgumentSpecifications.Cast<object>();
        }
    }
}