using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class Issue59_ArgDoWithReturns
    {
        public interface IStudent
        {
            int Id { set; }
            string Name { get; set; }
            string Say(string something);
        }

        [Test]
        public void Returns_and_arg_do_for_method()
        {
            var sub = Substitute.For<IStudent>();
            var lastArgUsedInCallToSayMethod = "";

            sub.Say(Arg.Do<string>(x => lastArgUsedInCallToSayMethod = x));
            sub.Say("hello").Returns("world");
            var resultOfHowdy = sub.Say("howdy");

            Assert.AreEqual("", resultOfHowdy);
            Assert.AreEqual("howdy", lastArgUsedInCallToSayMethod);
            Assert.AreEqual("world", sub.Say("hello"));
        }

        [Test]
        public void Returns_and_arg_do_for_property()
        {
            var sub = Substitute.For<IStudent>();
            var name = "";

            sub.Name = Arg.Do<string>(x => name = x);
            sub.Name = "Jane";
            sub.Name.Returns("Bob");

            Assert.AreEqual("Jane", name);
            Assert.AreEqual("Bob", sub.Name);
        }

        [Test]
        public void Returns_and_when_do()
        {
            var sub = Substitute.For<IStudent>();
            var name = "";

            sub.When(x => x.Name = Arg.Any<string>()).Do(x => name = x.Arg<string>());
            sub.Name = "Jane";

            sub.Name.Returns("Bob");

            Assert.AreEqual("Jane", name);
            Assert.AreEqual("Bob", sub.Name);
        }
    }
}