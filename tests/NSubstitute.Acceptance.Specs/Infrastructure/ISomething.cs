namespace NSubstitute.Acceptance.Specs.Infrastructure;

public interface ISomething
{
    int Count();
    int Anything(object stuff);
    string Echo(int i);
    string Say(string s);
    int Funky(float percentage, int amount, string label, ISomething something);
    int Add(int a, int b);
    string WithParams(int i, params string[] labels);
    int WithNullableArg(int? a);
    SomeClass SomeAction();
    SomeClass SomeActionWithParams(int i, string s);
    int SomeProperty { get; set; }
    int MethodWithRefParameter(int arg1, ref int arg2);
    int MethodWithMultipleRefParameters(int arg1, ref int arg2, ref int arg3);
    int MethodWithOutParameter(int arg1, out int arg2);
    int? NullableCount();
    int? NullableWithParams(int i, string s);

    object this[string key] { get; set; }
    Task Async();
    Task DoAsync(object stuff);
    Task<int> CountAsync();
    Task<int> AnythingAsync(object stuff);
    Task<string> EchoAsync(int i);
    Task<string> SayAsync(string s);
    Task<SomeClass> SomeActionAsync();
    Task<SomeClass> SomeActionWithParamsAsync(int i, string s);
    Task<int?> NullableCountAsync();
    Task<int?> NullableWithParamsAsync(int i, string s);

    ValueTask<int> CountValueTaskAsync();
    ValueTask<string> EchoValueTaskAsync(int i);
    ValueTask<int> AnythingValueTaskAsync(object stuff);
    ValueTask<string> SayValueTaskAsync(string s);
    ValueTask<SomeClass> SomeActionValueTaskAsync();
    ValueTask<SomeClass> SomeActionWithParamsValueTaskAsync(int i, string s);
    ValueTask<int?> NullableCountValueTaskAsync();
    ValueTask<int?> NullableCountValueTaskWithParamsAsync(int i, string s);
}