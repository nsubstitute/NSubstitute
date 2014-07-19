namespace NSubstitute.Core
{
    public interface ISubstituteContextResolver
    {
        ISubstituteContext ResolveFor(object substitute);
    }
}