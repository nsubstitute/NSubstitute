using NSubstitute.Core;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[Obsolete]
public class LegacyApiTests
{
    [Test]
    public void Set_pending_specification_should_be_returned_scenario1()
    {
        var call = Substitute.For<ICall>();
        var specificationInfo = PendingSpecificationInfo.FromLastCall(call);

        SubstitutionContext.Current.PendingSpecificationInfo = specificationInfo;
        var result = SubstitutionContext.Current.PendingSpecificationInfo;

        var encapsulatedObject = result.Handle<object>(x => x, x => x);
        Assert.That(encapsulatedObject, Is.SameAs(call));
    }

    [Test]
    public void Set_pending_specification_should_be_returned_scenario2()
    {
        var callSpec = Substitute.For<ICallSpecification>();
        var specificationInfo = PendingSpecificationInfo.FromCallSpecification(callSpec);

        SubstitutionContext.Current.PendingSpecificationInfo = specificationInfo;
        var result = SubstitutionContext.Current.PendingSpecificationInfo;

        var encapsulatedObject = result.Handle<object>(x => x, x => x);
        Assert.That(encapsulatedObject, Is.SameAs(callSpec));
    }

    [Test]
    public void Set_pending_specification_to_null_should_return_null()
    {
        SubstitutionContext.Current.PendingSpecificationInfo = null;

        var result = SubstitutionContext.Current.PendingSpecificationInfo;

        Assert.That(result, Is.Null);
    }

    [Test]
    public void Pending_specification_getter_should_not_remove_value()
    {
        var call = Substitute.For<ICall>();
        var specificationInfo = PendingSpecificationInfo.FromLastCall(call);

        SubstitutionContext.Current.PendingSpecificationInfo = specificationInfo;
        var result = SubstitutionContext.Current.PendingSpecificationInfo;
        result = SubstitutionContext.Current.PendingSpecificationInfo;
        result = SubstitutionContext.Current.PendingSpecificationInfo;

        var encapsulatedObject = result.Handle<object>(x => x, x => x);
        Assert.That(encapsulatedObject, Is.SameAs(call));
    }
}