using System;

namespace NSubstitute.Specs.SampleStructures
{
    public class Foo : IFoo
    {
        public virtual void Goo() {}
        public virtual string Bar(int aNumber, string aString) { return null; }

        public event EventHandler Boo;

        public void OnBoo(EventArgs e)
        {
            var handler = Boo;
            if (handler != null) handler(this, e);
        }
    }
}