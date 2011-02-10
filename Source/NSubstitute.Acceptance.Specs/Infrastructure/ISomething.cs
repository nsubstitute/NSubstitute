namespace NSubstitute.Acceptance.Specs
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
        object this[string key] { get; set; }
    }
}