#if NET4
using System.Threading.Tasks;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class ReturnValueInTask
    {
        [Test]
        public void Test()
        {
            var foo = Substitute.For<IFoo2>();
            foo.GetStringAsync().ReturnsValueAsTask("Bar");

            var result = foo.GetStringAsync().Result;

            Assert.AreEqual("Bar", result);
        }

        public interface IFoo2
        {
            Task<string> GetStringAsync();
        }
    }
}
#endif