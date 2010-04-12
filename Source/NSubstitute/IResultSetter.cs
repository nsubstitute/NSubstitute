namespace NSubstitute
{
    public interface IResultSetter
    {
        void SetResultForLastCall<T>(T valueToReturn);
    }
}