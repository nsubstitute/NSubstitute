using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue83_MethodsWithGenericStructConstraint
{
    public interface IService { T Get<T>(T arg) where T : struct; }

    [Test]
    public void TestGenericCalls()
    {
        var id = Guid.NewGuid();
        var service = Substitute.For<IService>();
        service.Get(id);
        service.Received().Get(id);
    }
}