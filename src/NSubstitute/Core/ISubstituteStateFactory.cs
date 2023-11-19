namespace NSubstitute.Core
{
    public interface ISubstituteStateFactory
    {
        ISubstituteState Create(ISubstituteFactory substituteFactory);
    }
}