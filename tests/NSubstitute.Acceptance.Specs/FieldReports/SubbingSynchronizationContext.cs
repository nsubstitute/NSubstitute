using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class SubbingSynchronizationContext
{
    [Test]
    public void Create_substitute_for_synchronization_context()
    {
        Substitute.For<SynchronizationContext>();
    }
}