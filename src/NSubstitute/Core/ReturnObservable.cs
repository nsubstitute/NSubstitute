namespace NSubstitute.Core;

internal sealed class ReturnObservable<T>(T? value) : IObservable<T?>
{
    public ReturnObservable() : this(default) { }

    public IDisposable Subscribe(IObserver<T?> observer)
    {
        observer.OnNext(value);
        observer.OnCompleted();

        return EmptyDisposable.Instance;
    }
}

internal sealed class EmptyDisposable : IDisposable
{
    public static IDisposable Instance { get; } = new EmptyDisposable();

    public void Dispose() { }
}