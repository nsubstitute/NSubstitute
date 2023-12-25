using NSubstitute.Compatibility;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;


/// <summary>
/// Can migrate from the old <see cref="Arg"/> matchers to <see cref="Arg.Compat"/> by putting a
/// <see cref="CompatArg"/> field named <c>Arg</c> in a fixture (or ideally in a project's
/// fixture base class). All old references to static Arg will then go through CompatArg instead.
/// 
/// To migrate back(once project switches to C#7+), just delete the CompatArgInstance field.
/// </summary>
public class MigratingToCompatArgs
{

    public class TestBaseClass
    {
        protected static readonly CompatArg Arg = CompatArg.Instance;
    }

    public interface IMessageServer
    {
        void SendMessage(int code, string description);
    }

    public class SampleTestFixture : TestBaseClass
    {
        [Test]
        public void ArgMatcherUsingBaseClass()
        {
            var sub = Substitute.For<IMessageServer>();

            sub.SendMessage(42, "meaning of life?");

            sub.Received().SendMessage(Arg.Is(42), Arg.Any<string>());
        }
    }
}
