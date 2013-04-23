using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class CallingIntoNewSubWithinReturns
    {
        public interface IFoo { string SomeString { get; set; } }
        public interface IBar { IFoo GetFoo(); }

        [Test]
        [Pending, Explicit]
        public void ShouldDetectTypeMismatchInReturns()
        {
            var sub = Substitute.For<IBar>();

            Assert.Throws<CouldNotSetReturnException>(() => 
                // GetFoo() called, then IPityTheFoo(), then Returns(..) is called.
                // This means Returns(..) tries to update the last called sub, 
                // which is IFoo.SomeString, not IBar.GetFoo(). 
                sub.GetFoo().Returns(IPityTheFoo())
            );
        }

        private IFoo IPityTheFoo()
        {
            var foo = Substitute.For<IFoo>();
            foo.SomeString = "call a property so static LastCalled is updated";
            return foo;
        }
    }
}