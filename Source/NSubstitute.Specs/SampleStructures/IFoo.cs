using System;

namespace NSubstitute.Specs.SampleStructures
{
    public interface IFoo
    {
        void Goo();
        event EventHandler Boo;
    }
}