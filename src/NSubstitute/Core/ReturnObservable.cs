namespace NSubstitute.Core;

internal class ReturnObservable<T>(T? value) : IObservable<T?>
{
    public ReturnObservable() : this(default) { }

    public IDisposable Subscribe(IObserver<T?> observer)
    {
        observer.OnNext(value);
        observer.OnCompleted();

        return EmptyDisposable.Instance;
    }
}

internal class EmptyDisposable : IDisposable
{
    public static IDisposable Instance { get; } = new EmptyDisposable();

    public void Dispose() { }
}