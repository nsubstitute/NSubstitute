using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class GenericArguments
{
    public interface ISomethingWithGenerics
    {
        void SomeAction<TState>(int level, TState state);
    }

    [Test]
    public void Any_matcher_works_with_AnyType()
    {
        ISomethingWithGenerics something = Substitute.For<ISomethingWithGenerics>();

        something.SomeAction(7, 3409);

        something.Received().SomeAction(Arg.Any<int>(), Arg.Any<Arg.AnyType>());
        something.Received().SomeAction(7, 3409);
    }

    [Test]
    public void When_Do_works_with_AnyType()
    {
        int? whenDoResult = null;
        bool whenDoCalled = false;
        ISomethingWithGenerics something = Substitute.For<ISomethingWithGenerics>();
        something
            .When(substitute => substitute.SomeAction(Arg.Any<int>(), Arg.Any<Arg.AnyType>()))
            .Do(info =>
            {
                whenDoResult = info.ArgAt<int>(1);
                whenDoCalled = true;
            });

        something.SomeAction(7, 3409);

        Assert.That(whenDoCalled, Is.True);
        Assert.That(whenDoResult, Is.EqualTo(3409));
    }

    [Test]
    public void ArgDo_works_with_AnyType()
    {
        string argDoResult = null;
        ISomethingWithGenerics something = Substitute.For<ISomethingWithGenerics>();
        something.SomeAction(Arg.Any<int>(), Arg.Do<Arg.AnyType>(a => argDoResult = ">>" + ((int)a).ToString("P", CultureInfo.InvariantCulture)));

        something.SomeAction(7, 3409);

        Assert.That(argDoResult, Is.EqualTo(">>340,900.00 %"));
    }
}