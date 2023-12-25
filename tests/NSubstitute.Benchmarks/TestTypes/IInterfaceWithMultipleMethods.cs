namespace NSubstitute.Benchmarks.TestTypes;

public interface IInterfaceWithMultipleMethods
{
    int MethodWithArg(int arg);
    int MethodWithRefArg(ref int arg);
    int MethodWithOutArg(out int arg);
}