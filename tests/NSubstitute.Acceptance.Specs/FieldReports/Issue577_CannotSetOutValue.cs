using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue577_CannotSetOutValueWhenPassedValueIsOfIncompatibleType
{
    [Test]
    public void Should_reassign_out_value_when_type_is_incompatible_with_passed_value_original()
    {
        // Arrange
        var keyValueDatabase = Substitute.For<IDatabase>();

        var a = Substitute.For<IKeyValueDouble>();
        var b = Substitute.For<IKeyValueString>();

        keyValueDatabase.GetKeyValue(
            Arg.Any<string>(),
            out Arg.Any<IKeyValue>()
        ).Returns(info =>
        {
            var id = (string)info[0];
            if (id != null && id == "a")
            {
                info[1] = a;
            }
            else
            {
                info[1] = b;
            }

            return true;
        });

        // Act
        IKeyValue outVal;
        keyValueDatabase.GetKeyValue("a", out outVal);
        keyValueDatabase.GetKeyValue("b", out outVal);
    }

    [Test]
    public void Should_reassign_out_value_when_type_is_incompatible_with_passed_value()
    {
        // Arrange
        var db = Substitute.For<IDatabase>();

        var a = Substitute.For<IKeyValueDouble>();
        var b = Substitute.For<IKeyValueString>();

        db.GetKeyValue(Arg.Any<string>(), out Arg.Any<IKeyValue>())
            .Returns(c =>
            {
                c[1] = b;
                return true;
            });

        // Act
        IKeyValue result = a;
        db.GetKeyValue("42", out result);
        Assert.That(result, Is.EqualTo(b));
    }

    [Test]
    public void Should_reassign_ref_value_when_type_is_incompatible_with_passed_value()
    {
        // Arrange
        var db = Substitute.For<IDatabase>();

        var a = Substitute.For<IKeyValueDouble>();
        var b = Substitute.For<IKeyValueString>();

        db.GetKeyValueRef(Arg.Any<string>(), ref Arg.Any<IKeyValue>())
            .Returns(c =>
            {
                c[1] = b;
                return true;
            });

        // Act
        IKeyValue result = a;
        db.GetKeyValueRef("42", ref result);
        Assert.That(result, Is.EqualTo(b));
    }

    public interface IKeyValue
    {
        string ID { get; set; }
    }

    public interface IKeyValueDouble : IKeyValue
    {
        double Value { get; set; }
    }

    public interface IKeyValueString : IKeyValue
    {
        string Value { get; set; }
    }

    public interface IDatabase
    {
        bool GetKeyValue(string id, out IKeyValue value);
        bool GetKeyValueRef(string id, ref IKeyValue value);
    }
}
