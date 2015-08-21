using System;

namespace NSubstitute.Core
{
    public class ResultsForType : IResultsForType
    {

        class ResultForTypeSpec
        {
            private readonly Type _type;
            private readonly IReturn _resultToReturn;

            public ResultForTypeSpec(Type type, IReturn resultToReturn)
            {
                _type = type;
                _resultToReturn = resultToReturn;
            }

            public bool IsResultFor(ICall call)
            {
                return call.GetReturnType() == _type;
            }

            public object GetResult(CallInfo callInfo)
            {
                return _resultToReturn.ReturnFor(callInfo);
            }
        }
    }
}