#if NET45
using System.Threading.Tasks;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    [TestFixture]
    public class Issue282_MultipleReturnValuesParallelism
    {
        public interface IFoo
        {
            string Foo();
        }

        [Test]
        [Ignore("#282")]
        public async void ReturnsMultipleValuesInParallel()
        {
            var ret1 = "One";
            var ret2 = "Two";

            var substitute = Substitute.For<IFoo>();
            substitute.Foo().Returns(ret1, ret2);

            var runningTask1 = Task.Run(() => substitute.Foo());
            var runningTask2 = Task.Run(() => substitute.Foo());

            var results = await Task.WhenAll(runningTask1, runningTask2);

            Assert.Contains(ret1, results);
            Assert.Contains(ret2, results);
        }
    }
}
#endif
