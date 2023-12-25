using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

using NestedGeneric = Issue515_OutOfRangeExceptionForNestedGenericTypes.Foo<int, string>.Intermediate.Nested<List<byte>>;
using NestedNonGeneric = Issue515_OutOfRangeExceptionForNestedGenericTypes.Bar.Intermediate.Nested;

public class Issue515_OutOfRangeExceptionForNestedGenericTypes
{
    [Test]
    public void ShouldCorrectlyFormatNestedGenericTypeName()
    {
        var sub = Substitute.For<IConsumer>();
        sub.ConsumeGeneric(new NestedGeneric());

        var ex = Assert.Throws<ReceivedCallsException>(
            () => sub.Received().ConsumeGeneric(null));
        Assert.That(ex.Message, Contains.Substring(
            "ConsumeGeneric(*Issue515_OutOfRangeExceptionForNestedGenericTypes+Foo<Int32, String>+Intermediate+Nested<List<Byte>>*)"));
    }

    [Test]
    public void ShouldCorrectlyFormatNestedNonGenericTypeName()
    {
        var sub = Substitute.For<IConsumer>();
        sub.ConsumeNonGeneric(new NestedNonGeneric());

        var ex = Assert.Throws<ReceivedCallsException>(
            () => sub.Received().ConsumeNonGeneric(null));
        Assert.That(ex.Message, Contains.Substring(
            "ConsumeNonGeneric(*Issue515_OutOfRangeExceptionForNestedGenericTypes+Bar+Intermediate+Nested*)"));
    }

    public interface IConsumer
    {
        void ConsumeGeneric(NestedGeneric arg);
        void ConsumeNonGeneric(NestedNonGeneric arg);
    }

    public class Foo<T1, T2>
    {
        public class Intermediate
        {
            public class Nested<T3> where T3 : IEnumerable<byte>
            {
                public int Baz { get; } = 42;
            }
        }
    }

    public class Bar
    {
        public class Intermediate
        {
            public class Nested
            {
                public int Baz { get; } = 42;
            }
        }
    }
}