namespace NSubstitute.Core
{
    public interface ICallResults
    {
        void SetResult(ICallSpecification callSpecification, IReturn result);
        bool HasResultFor(ICall call);
        object GetResult(ICall call);
        /// <summary>
        /// Gets the result by spec similarity.
        /// </summary>
        /// <param name="callSpecification">The call specification.</param>
        /// <returns></returns>
        IReturn GetResultBySpecSimilarity(ICallSpecification callSpecification);
    }
}