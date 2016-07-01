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

        object this[string key] { get; set; }
#if (NET45 || NET4 || NETSTANDARD1_5)
        System.Threading.Tasks.Task Async();
        System.Threading.Tasks.Task<int> CountAsync();
        System.Threading.Tasks.Task<string> EchoAsync(int i);
#endif
    }
}