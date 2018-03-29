namespace NSubstitute.Core
{
    public interface ISubstituteStateFactory
    {
        ISubstituteState Create(SubstituteConfig config, ISubstituteFactory substituteFactory);
    }
}