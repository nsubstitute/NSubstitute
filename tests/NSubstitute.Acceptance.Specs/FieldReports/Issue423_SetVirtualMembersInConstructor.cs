using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue423_SetVirtualMembersInConstructor
{
    public class TypeWithVirtualMembers
    {
        public virtual string Property { get; set; }
        public virtual string ReadOnlyProp { get; }
        public virtual string IndirectlyInitProperty { get; set; }
        protected virtual string ProtectedProp { get; set; }

        public TypeWithVirtualMembers(string propertyValue)
        {
            Property = propertyValue;
            ReadOnlyProp = propertyValue;
            ProtectedProp = propertyValue;
            InitPropertyIndirectly(propertyValue);

            SomeVirtualMethod();
        }

        public virtual void SomeVirtualMethod()
        {
        }

        public string GetProtectedPropertyValue() => ProtectedProp;

        public void InitPropertyIndirectly(string value)
        {
            IndirectlyInitProperty = value;
        }
    }

    [Test]
    public void ShouldSetVirtualPropertiesForPartialSubstitution()
    {
        var propValue = "42";

        var subs = Substitute.ForPartsOf<TypeWithVirtualMembers>(propValue);

        Assert.That(subs.Property, Is.EqualTo(propValue));
        Assert.That(subs.ReadOnlyProp, Is.EqualTo(propValue));
        Assert.That(subs.GetProtectedPropertyValue(), Is.EqualTo(propValue));
    }

    [Test]
    public void ShouldInvokeVirtualMembersCalledInConstructor()
    {
        var propValue = "42";

        var subs = Substitute.ForPartsOf<TypeWithVirtualMembers>(propValue);

        Assert.That(subs.IndirectlyInitProperty, Is.EqualTo(propValue));
    }

    [Test]
    public void ShouldNotCountInvokedMethodInConstructor()
    {
        var subs = Substitute.ForPartsOf<TypeWithVirtualMembers>("42");

        subs.DidNotReceive().SomeVirtualMethod();
    }

    [Test]
    public void VirtualPropertyValuesShouldNotBePreservedForRegularSubstitution()
    {
        var propValue = "42";

        var subs = Substitute.For<TypeWithVirtualMembers>(propValue);

        Assert.That(subs.Property, Is.Not.EqualTo(propValue));
        Assert.That(subs.ReadOnlyProp, Is.Not.EqualTo(propValue));
        Assert.That(subs.GetProtectedPropertyValue(), Is.Not.EqualTo(propValue));
    }
}