using NUnit.Framework;

namespace Na { public delegate void DoNothing(); }
namespace Ns { public delegate void DoNothing(); }
namespace Nt { public delegate void DoNothing(); }
namespace Nz { public delegate void DoNothing(); }

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class Issue631_NamespaceDelegate
    {
        [Test] public void Na() { Substitute.For<Na.DoNothing>(); }
        [Test] public void Ns() { Substitute.For<Ns.DoNothing>(); }
        [Test] public void Nt() { Substitute.For<Nt.DoNothing>(); }
        [Test] public void Nz() { Substitute.For<Nz.DoNothing>(); }
    }
}