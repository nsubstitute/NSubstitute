namespace NSubstitute.Acceptance.Specs.Infrastructure;

public abstract class AnotherClass
{
    protected abstract string ProtectedMethod();

    protected abstract string ProtectedMethod(int i);

    protected abstract string ProtectedMethod(string msg, int i, char j);

    protected abstract void ProtectedMethodWithNoReturn();

    protected abstract void ProtectedMethodWithNoReturn(int i);

    protected abstract void ProtectedMethodWithNoReturn(string msg, int i, char j);

    public abstract void PublicVirtualMethod();

    protected void ProtectedNonVirtualMethod()
    { }

    public string DoWork()
    {
        return ProtectedMethod();
    }

    public string DoWork(int i)
    {
        return ProtectedMethod(i);
    }

    public string DoWork(string msg, int i, char j)
    {
        return ProtectedMethod(msg, i, j);
    }

    public void DoVoidWork()
    {
        ProtectedMethodWithNoReturn();
    }

    public void DoVoidWork(int i)
    {
        ProtectedMethodWithNoReturn(i);
    }

    public void DoVoidWork(string msg, int i, char j)
    {
        ProtectedMethodWithNoReturn(msg, i, j);
    }
}