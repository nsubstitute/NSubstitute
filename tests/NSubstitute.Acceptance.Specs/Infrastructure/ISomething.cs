namespace NSubstitute.Acceptance.Specs.Infrastructure
{
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

        object this[string key] { get; set; }
        System.Threading.Tasks.Task Async();
        System.Threading.Tasks.Task DoAsync(object stuff);
        System.Threading.Tasks.Task<int> CountAsync();
        System.Threading.Tasks.Task<int> AnythingAsync(object stuff);
        System.Threading.Tasks.Task<string> EchoAsync(int i);
        System.Threading.Tasks.Task<string> SayAsync(string s);
        System.Threading.Tasks.Task<SomeClass> SomeActionAsync();
        System.Threading.Tasks.Task<SomeClass> SomeActionWithParamsAsync(int i, string s);

        System.Threading.Tasks.ValueTask<int> CountValueTaskAsync();
        System.Threading.Tasks.ValueTask<string> EchoValueTaskAsync(int i);
        System.Threading.Tasks.ValueTask<int> AnythingValueTaskAsync(object stuff);
        System.Threading.Tasks.ValueTask<string> SayValueTaskAsync(string s);
        System.Threading.Tasks.ValueTask<SomeClass> SomeActionValueTaskAsync();
        System.Threading.Tasks.ValueTask<SomeClass> SomeActionWithParamsValueTaskAsync(int i, string s);
    }
}