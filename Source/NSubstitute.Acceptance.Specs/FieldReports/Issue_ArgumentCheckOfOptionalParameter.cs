using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class Issue_ArgumentCheckOfOptionalParameter
    {
        public interface IInterface
        {
            void MethodWithOptionalParameter(object obligatory, object optional = null);
        }

        [Test]
        [Pending, Explicit]
        public void PassArgumentCheckForOptionalParameter()
        {
            var substitute = Substitute.For<IInterface>();
            substitute.MethodWithOptionalParameter(new object());
            substitute.ReceivedWithAnyArgs().MethodWithOptionalParameter(Arg.Any<object>());
        }
    }
}