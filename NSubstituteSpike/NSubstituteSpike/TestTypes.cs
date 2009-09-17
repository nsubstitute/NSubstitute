using System;

namespace NSubstituteSpike
{
    public interface IFoo
    {
        int Calculate();
        string Concat(string a, string b);
        Bar CreateBar();
        Guid SomeId { get; set; }
    }
    public class Bar { }
}