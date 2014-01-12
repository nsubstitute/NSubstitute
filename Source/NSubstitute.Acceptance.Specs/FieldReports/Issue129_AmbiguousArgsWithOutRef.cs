using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class Issue129_AmbiguousArgsWithOutRef
    {
        public interface ITestInterface
        {
            bool DoStuff(string input, out string output);
        }

        [Test]
        public void Test()
        {
            string someType;
            var substitute = Substitute.For<ITestInterface>();
            substitute.DoStuff(Arg.Any<string>(), out someType).ReturnsForAnyArgs(info =>
            {
                info[1] = "test";
                return true;
            });

            string outString;
            var result =substitute.DoStuff("a", out outString);

            Assert.That(result);
            Assert.That(outString, Is.EqualTo("test"));
        }

        [Test]
        public void WorkAround()
        {
            string someType;
            var substitute = Substitute.For<ITestInterface>();
            var arg0 = Arg.Any<string>();
            var arg1 = Arg.Any<string>();
            substitute.DoStuff(arg0, out someType).ReturnsForAnyArgs(info =>
            {
                info[1] = "test";
                return true;
            });

            string outString;
            var result =substitute.DoStuff("a", out outString);

            Assert.That(result);
            Assert.That(outString, Is.EqualTo("test"));
        }
    }
}