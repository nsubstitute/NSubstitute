using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public interface IArgumentsFormatter
    {
        string Format(IEnumerable<IArgumentFormatInfo> argumentFormatInfos);
    }
}