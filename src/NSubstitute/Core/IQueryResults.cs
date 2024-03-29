namespace NSubstitute.Core;

public interface IQueryResults
{
    IEnumerable<ICall> MatchingCallsInOrder();
    IEnumerable<CallSpecAndTarget> QuerySpecification();
}