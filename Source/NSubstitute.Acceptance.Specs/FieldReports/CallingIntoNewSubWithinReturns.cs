using System;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class CallingIntoNewSubWithinReturns
    {
        public interface IFoo { string SomeString { get; set; } }
        public interface IBar { IFoo GetFoo(); }
        public interface IZap { int Num { get; set; } }

        [Test]
        public void ShouldDetectTypeMismatchInReturns()
        {
            var sub = Substitute.For<IBar>();

            var ex = 
                Assert.Throws<CouldNotSetReturnDueToTypeMismatchException>(() =>
                    // GetFoo() called, then IPityTheFoo(), then Returns(..) is called.
                    // This means Returns(..) tries to update the last called sub, 
                    // which is IFoo.SomeString, not IBar.GetFoo(). 
                    sub.GetFoo().Returns(IPityTheFoo())
                );

            Assert.That(ex.Message, Is.StringStarting("Can not return value of type "));
        }

        private IFoo IPityTheFoo()
        {
            var foo = Substitute.For<IFoo>();
            foo.SomeString = "call a property so static LastCalled is updated";
            return foo;
        }

        [Test]
        public void ShouldDetectedWhenNestedReturnsClearsLastCallRouter()
        {
            var sub = Substitute.For<IBar>();

            Assert.Throws<CouldNotSetReturnDueToNoLastCallException>(() => 
                sub.GetFoo().Returns(CreateFooAndCallReturns())
            );
        }

        private IFoo CreateFooAndCallReturns()
        {
            var foo = Substitute.For<IFoo>();
            foo.SomeString.Returns("nested set");
            return foo;
        }

        [Test]
        public void ShouldDetectTypeMismatchWhenNullIsInvolved()
        {
            var sub = Substitute.For<IBar>();

            var ex = 
                Assert.Throws<CouldNotSetReturnDueToTypeMismatchException>(() => 
                    sub.GetFoo().Returns(DoEvilAndReturnNullRef())
                );

            Assert.That(ex.Message, Is.StringStarting("Can not return null for"));
        }

        private IFoo DoEvilAndReturnNullRef()
        {
            var zap = Substitute.For<IZap>();
            zap.Num = 2;
            return null;
        }
    }
}
