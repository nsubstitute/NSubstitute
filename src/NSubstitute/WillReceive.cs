using NSubstitute.Core;

namespace NSubstitute;

public static class WillReceive
{
    public static WillReceiveExpectation InOrder(Action calls)
    {
        return new WillReceiveExpectation(
            callSpecificationFactory: SubstitutionContext.Current.CallSpecificationFactory,
            buildExpectationsAction: calls);
    }
}