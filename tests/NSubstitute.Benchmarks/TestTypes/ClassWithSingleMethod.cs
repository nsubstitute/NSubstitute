namespace NSubstitute.Benchmarks.TestTypes;

public class ClassWithSingleMethod
{
    public virtual int IntMethod(string arg) => 0;

    public virtual void VoidMethod(string arg)
    {
    }
}