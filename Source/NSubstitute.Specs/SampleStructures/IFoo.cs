using System;

namespace NSubstitute.Specs.SampleStructures
{
    public interface IFoo
    {
        void Goo();
        string Bar(int aNumber, string aString);
        event EventHandler Boo;
    }
}