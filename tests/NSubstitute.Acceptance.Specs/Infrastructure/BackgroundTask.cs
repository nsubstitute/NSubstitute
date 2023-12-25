namespace NSubstitute.Acceptance.Specs.Infrastructure;

public class BackgroundTask
{
    readonly Action _start;
    readonly Action _await;
    private Exception _exception;
    public BackgroundTask(Action action)
    {
        var thread = new Thread(() =>
                                    {
                                        try { action(); }
                                        catch (Exception ex) { _exception = ex; }
                                    });
        _await = () => { thread.Join(); ThrowIfError(); };
        _start = () => thread.Start();
    }
    void ThrowIfError() { if (_exception != null) throw new Exception("Thread threw", _exception); }

    public static void StartAll(BackgroundTask[] tasks) { foreach (var task in tasks) { task._start(); } }
    public static void AwaitAll(BackgroundTask[] tasks) { foreach (var task in tasks) { task._await(); } }
}