using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue57_SettingVirtualPropertyInCtorCausesReturnsToUseAutoSub
{
    public class MyEntity
    {
        public MyEntity() { Name = "Name1"; }
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
    }

    public interface IEntityRepository { MyEntity Get(Guid id); }

    [Test]
    public void TestGetFromRepository()
    {
        var repository = Substitute.For<IEntityRepository>();
        var fakeEntity = new MyEntity { Id = Guid.NewGuid() };

        repository.Get(Arg.Any<Guid>()).Returns(fakeEntity);

        var result = repository.Get(fakeEntity.Id);
        Assert.That(result, Is.SameAs(fakeEntity));
        Assert.That(result.Id, Is.EqualTo(fakeEntity.Id));
    }

    [Test]
    public void TestGetUsingAutoSubs()
    {
        var repository = Substitute.For<IEntityRepository>();
        var fakeEntity = repository.Get(Arg.Any<Guid>());
        fakeEntity.Id = Guid.NewGuid();

        var result = repository.Get(fakeEntity.Id);

        Assert.That(result, Is.SameAs(fakeEntity));
        Assert.That(result.Id, Is.EqualTo(fakeEntity.Id));
    }
}
