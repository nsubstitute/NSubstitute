using NUnit.Framework;
using System.Threading.Tasks;

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
        public void ReturnsMultipleValuesInParallel()
        {
            var ret1 = "One";
            var ret2 = "Two";

            var substitute = Substitute.For<IFoo>();
            substitute.Foo().Returns(ret1, ret2);

#if NET40
            var runningTask1 = TaskEx.Run(() => substitute.Foo());
            var runningTask2 = TaskEx.Run(() => substitute.Foo());
            var results = TaskEx.WhenAll(runningTask1, runningTask2).Result;
#else
            var runningTask1 = Task.Run(() => substitute.Foo());
            var runningTask2 = Task.Run(() => substitute.Foo());
            var results = Task.WhenAll(runningTask1, runningTask2).Result;
#endif

            Assert.Contains(ret1, results);
            Assert.Contains(ret2, results);
        }
    }
}
