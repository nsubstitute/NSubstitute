namespace NSubstitute.Acceptance.Specs.Infrastructure;

public interface ISomethingWithGenericMethods
{
    void Log<TState>(int level, TState state);
    string Format<T>(T state);
}