using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class MatchingDerivedTypesForGenerics
{
    IGenMethod _sub;

    [SetUp]
    public void Setup()
    {
        _sub = Substitute.For<IGenMethod>();
    }

    [Test]
    public void Calls_to_generic_types_with_derived_parameters_should_be_matched()
    {
        _sub.Call(new GMParam1());
        _sub.Call(new GMParam2());

        _sub.Received(2).Call(Arg.Any<IGMParam>());
    }

    [Test]
    public void Calls_to_generic_types_expecting_specific_argument_type()
    {
        _sub.Call(new GMParam1());
        _sub.Call(new GMParam2());

        _sub.Received(1).Call(Arg.Any<GMParam2>());
    }

    [Test]
    public void Calls_to_generic_types_expecting_specific_argument()
    {
        var gmParam1 = new GMParam1();
        _sub.Call(gmParam1);
        _sub.Call(new GMParam2());

        _sub.Received(1).Call(gmParam1);
    }

    [Test]
    public void Stub_generic_method()
    {
        _sub.IntCall(Arg.Any<IGMParam>()).Returns(42);

        Assert.That(_sub.IntCall(new GMParam2()), Is.EqualTo(42));
    }

    [Test]
    public void Stub_generic_method_with_specific_subtype()
    {
        _sub.IntCall(Arg.Any<GMParam2>()).Returns(42);

        Assert.That(_sub.IntCall(new GMParam2()), Is.EqualTo(42));
        Assert.That(_sub.IntCall(new GMParam1()), Is.EqualTo(default(int)));
    }

    public interface IGenMethod
    {
        void Call<T>(T param) where T : IGMParam;
        int IntCall<T>(T param) where T : IGMParam;
    }
    public interface IGMParam { }
    public class GMParam1 : IGMParam { }
    public class GMParam2 : IGMParam { }
}

[TestFixture]
public class Calls_to_members_on_generic_interface
{
    [Test]
    public void Calls_to_members_on_generic_interfaces_match_based_on_type_compatibility()
    {
        var sub = Substitute.For<IGenInterface<IGMParam>>();
        sub.Call(new GMParam1());
        sub.Call(new GMParam2());
        sub.Call(new GMParam3());

        sub.Received(2).Call(Arg.Any<IGMParam>());
        sub.Received(1).Call(Arg.Any<GMParam3>());
    }

    [Test]
    [Pending, Explicit]
    public void Potentially_ambiguous_matches()
    {
        var sub = Substitute.For<IGenInterface<IGMParam>>();
        sub.Call(new GMParam1());
        sub.Call(new GMParam2());
        sub.Call(new GMParam4());

        sub.Received(2).Call(Arg.Any<IGMParam>());
        sub.Received(1).Call(Arg.Any<GMParam4>());
    }

    public interface IGenInterface<T>
    {
        void Call(T param);
        void Call(GMParam3 param);
        void Call(GMParam4 param);
    }

    public interface IGMParam { }
    public class GMParam1 : IGMParam { }
    public class GMParam2 : IGMParam { }
    public class GMParam3 { }
    public class GMParam4 : IGMParam { }
}
