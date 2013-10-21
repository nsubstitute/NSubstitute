using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class TestRefAndOutArgAnyWithWhenDo
    {
        [Test]
        public void WhenDo_GivenRefParamWithArgAnyString_ShouldCallDo()
        {
            var substitute = Substitute.For<IDummy>();
            var p = new DummyLauncher();
            var counter = 0;
            var strArg = Arg.Any<string>();
            substitute.When(x => x.SomeMethodWithARefArg(ref strArg)).Do(x => counter++);
            p.LaunchTheRefDummy(substitute);
            Assert.That(counter, Is.EqualTo(1));
        }

        [Test]
        public void WhenDo_GivenRefParamWithActualString_ShouldCallDo()
        {
            var substitute = Substitute.For<IDummy>();
            var p = new DummyLauncher();
            var counter = 0;
            var strArg = "hello";
            substitute.When(x => x.SomeMethodWithARefArg(ref strArg)).Do(x => counter++);
            p.LaunchTheRefDummy(substitute);
            Assert.That(counter, Is.EqualTo(1));
        }

        [Test]
        public void WhenDo_GivenRefParamWithDifferentString_ShouldNotCallDo()
        {
            var substitute = Substitute.For<IDummy>();
            var p = new DummyLauncher();
            var counter = 0;
            var strArg = "what";
            substitute.When(x => x.SomeMethodWithARefArg(ref strArg)).Do(x => counter++);
            p.LaunchTheRefDummy(substitute);
            Assert.That(counter, Is.EqualTo(0));
        }

        [Test]
        public void WhenDo_GivenOutParamWithArgAnyString_ShouldCallDo()
        {
            var substitute = Substitute.For<IDummy>();
            var p = new DummyLauncher();
            var counter = 0;
            var strArg = Arg.Any<string>();
            substitute.When(x => x.SomeMethodWithAnOutArg(out strArg)).Do(x => counter++);
            p.LaunchTheOutDummy(substitute);
            Assert.That(counter, Is.EqualTo(1));
        }

        [Test]
        public void WhenDo_GivenOutParamWithActualString_ShouldCallDo()
        {
            var substitute = Substitute.For<IDummy>();
            var p = new DummyLauncher();
            var counter = 0;
            var strArg = "hello";
            substitute.When(x => x.SomeMethodWithAnOutArg(out strArg)).Do(x => counter++);
            p.LaunchTheOutDummy(substitute);
            Assert.That(counter, Is.EqualTo(1));
        }
        
        [Test]
        public void WhenDo_GivenOutParamWithDifferentString_ShouldNotCallDo()
        {
            var substitute = Substitute.For<IDummy>();
            var p = new DummyLauncher();
            var counter = 0;
            var strArg = "";
            substitute.When(x => x.SomeMethodWithAnOutArg(out strArg)).Do(x => counter++);
            p.LaunchTheOutDummy(substitute);
            Assert.That(counter, Is.EqualTo(0));
        }

        public class DummyLauncher
        {
            public void LaunchTheRefDummy(IDummy dummy)
            {
                var test = "hello";
                dummy.SomeMethodWithARefArg(ref test);
            }

            public void LaunchTheOutDummy(IDummy dummy)
            {
                var test = "hello";
                dummy.SomeMethodWithAnOutArg(out test);
            }
        }

        public interface IDummy
        {
            bool SomeMethodWithARefArg(ref string str);
            bool SomeMethodWithAnOutArg(out string str);
        }
    }
}
