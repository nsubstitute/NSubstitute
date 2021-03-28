using System;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class ReturnFromBase
    {
        public class Sample
        {
            public virtual string RepeatButLouder(string s) => s + "!";
            public virtual void VoidMethod() { }
        }

        public abstract class SampleWithAbstractMethod
        {
            public abstract string NoBaseImplementation();
        }

        public interface ISample
        {
            string InterfaceMethod();
        }

        [Test]
        public void UseBaseInReturn() {
            var sub = Substitute.For<Sample>();
            sub.RepeatButLouder(Arg.Any<string>()).Returns(x => x.BaseResult() + "?");

            Assert.AreEqual("Hi!?", sub.RepeatButLouder("Hi"));
        }

        [Test]
        public void CallWithNoBaseImplementation() {
            var sub = Substitute.For<SampleWithAbstractMethod>();
            sub.NoBaseImplementation().Returns(x => x.BaseResult());

            Assert.Throws<NoBaseImplementationException>(() =>
                sub.NoBaseImplementation()
            );
        }

        [Test]
        public void CallBaseForInterface() {
            var sub = Substitute.For<ISample>();
            sub.InterfaceMethod().Returns(x => x.BaseResult());
            Assert.Throws<NoBaseImplementationException>(() =>
                sub.InterfaceMethod()
            );
        }
    }
}