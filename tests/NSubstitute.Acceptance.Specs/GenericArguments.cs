using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class GenericArguments
{
    public interface ISomethingWithGenerics
    {
        void Log<TState>(int level, TState state);
    }

    [Test]
    public void Return_result_for_any_argument()
    {
        string argDoResult = null;
        int? whenDoResult = null;
        bool whenDoCalled = false;

        ISomethingWithGenerics something = Substitute.For<ISomethingWithGenerics>();
        something.Log(Arg.Any<int>(), Arg.DoForAny(a => argDoResult = a.ToString()));
        something
            .When(substitute => substitute.Log(Arg.Any<int>(), Arg.Any<Arg.AnyType>()))
            .Do(info =>
            {
                whenDoResult = info.ArgAt<int>(1);
                whenDoCalled = true;
            });

        something.Log(7, 3409);

        something.Received().Log(Arg.Any<int>(), Arg.Any<Arg.AnyType>());
        something.Received().Log(7, 3409);
        Assert.That(whenDoCalled, Is.True);
        Assert.That(argDoResult, Is.EqualTo("3409"));
        Assert.That(whenDoResult, Is.EqualTo(3409));
    }

    [Test]
    public void Throw_with_Do_AnyType()
    {
        ISomethingWithGenerics something = Substitute.For<ISomethingWithGenerics>();

        Assert.Throws<DoAnyTypeException>(() =>
            something.Log(Arg.Any<int>(), Arg.Do<Arg.AnyType>(a => _ = a.ToString())));
    }
}