using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class ObjectArgumentParamsExtractor : ArgumentParamsExtractor
    {
        protected override IEnumerable<object> GetExtractedParamsArguments(IEnumerable<object> arguments)
        {
            object[] paramsArgument = arguments.Last() as object[];

            if (paramsArgument == null)
            {
                throw new ArgumentException("Last argument is not an array.", "arguments");
            }

            return paramsArgument;
        }
    }
}