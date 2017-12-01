using System.Collections.Generic;
using System.Linq;
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

            var runningTask1 = Task.Factory.StartNew(() => substitute.Foo());
            var runningTask2 = Task.Factory.StartNew(() => substitute.Foo());
            var tasks = new[] {runningTask1, runningTask2};

            var results = new List<string>();
            Task.Factory.StartNew(() => Task.WaitAll(tasks));
            results.AddRange(tasks.Select(t => t.Result));

            Assert.Contains(ret1, results);
            Assert.Contains(ret2, results);
        }
    }
}
