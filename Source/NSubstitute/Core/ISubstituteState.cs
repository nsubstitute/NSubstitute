using System;

namespace NSubstitute.Core
{
    public interface ISubstituteState
    {
        object FindInstanceFor(Type type, object[] additionalArguments);
    }
}