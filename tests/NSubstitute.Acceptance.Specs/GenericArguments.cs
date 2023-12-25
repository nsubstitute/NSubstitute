using System.Collections;
using System.Globalization;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class GenericArguments
{
    public interface ISomethingWithGenerics
    {
        void SomeAction<TState>(int level, TState state);
        string SomeFunction<TState>(int level, TState state);
        void SomeActionWithGenericConstraints<TState>(int level, TState state) where TState : IEnumerable<int>;
        string SomeFunctionWithGenericConstraints<TState>(int level, TState state) where TState : IEnumerable<int>;
    }

    public abstract class MyAnyType : IEnumerable<int>, Arg.AnyType
    {
        public abstract IEnumerator<int> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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

    [Test]
    public void Is_matcher_works_with_AnyType()
    {
        ISomethingWithGenerics something = Substitute.For<ISomethingWithGenerics>();

        something.SomeFunction(Arg.Any<int>(), Arg.Is<Arg.AnyType>(a => (int)a == 3409)).Returns("matched");

        var result = something.SomeFunction(7, 3409);

        Assert.That(result, Is.EqualTo("matched"));
    }

    [Test]
    public void Any_matcher_works_with_AnyType_and_constraints()
    {
        ISomethingWithGenerics something = Substitute.For<ISomethingWithGenerics>();
        var state = new[] { 3409 };
        something.SomeActionWithGenericConstraints(7, state);

        something.Received().SomeActionWithGenericConstraints(Arg.Any<int>(), Arg.Any<MyAnyType>());
        something.Received().SomeActionWithGenericConstraints(7, state);
    }

    [Test]
    public void When_Do_works_with_AnyType_and_constraints()
    {
        int[] whenDoResult = null;
        bool whenDoCalled = false;
        ISomethingWithGenerics something = Substitute.For<ISomethingWithGenerics>();
        something
            .When(substitute => substitute.SomeActionWithGenericConstraints(Arg.Any<int>(), Arg.Any<MyAnyType>()))
            .Do(info =>
            {
                whenDoResult = info.ArgAt<int[]>(1);
                whenDoCalled = true;
            });

        var expected = new[] { 3409 };
        something.SomeActionWithGenericConstraints(7, expected);

        Assert.That(whenDoCalled, Is.True);
        Assert.That(whenDoResult, Is.EqualTo(expected));
    }

    [Test]
    public void ArgDo_works_with_AnyType_and_constraints()
    {
        string argDoResult = null;
        ISomethingWithGenerics something = Substitute.For<ISomethingWithGenerics>();
        something.SomeActionWithGenericConstraints(Arg.Any<int>(), Arg.Do<MyAnyType>(a => argDoResult = ">>" + ((int[])a)[0].ToString("P", CultureInfo.InvariantCulture)));

        something.SomeActionWithGenericConstraints(7, new[] { 3409 });

        Assert.That(argDoResult, Is.EqualTo(">>340,900.00 %"));
    }

    [Test]
    public void Is_matcher_works_with_AnyType_and_constraints()
    {
        ISomethingWithGenerics something = Substitute.For<ISomethingWithGenerics>();

        something.SomeFunctionWithGenericConstraints(Arg.Any<int>(), Arg.Is<MyAnyType>(a => ((int[])a)[0] == 3409)).Returns("matched");

        var result = something.SomeFunctionWithGenericConstraints(7, new[] { 3409 });

        Assert.That(result, Is.EqualTo("matched"));
    }
}