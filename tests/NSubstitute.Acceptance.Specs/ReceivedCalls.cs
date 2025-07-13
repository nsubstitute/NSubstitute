using NSubstitute.Exceptions;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class ReceivedCalls
{
    private ICar _car;
    const int Rpm = 7000;
    private static readonly object[] Luggage = [new object(), new object()];
    private static readonly DateTime[] ServiceDates = [new DateTime(2001, 01, 01), new DateTime(2002, 02, 02)];

    [SetUp]
    public void SetUp()
    {
        _car = Substitute.For<ICar>();
    }

    [Test]
    public void Check_when_call_was_received()
    {
        _car.Rev();

        _car.Received().Rev();
    }

    [Test]
    public void Throw_when_expected_call_was_not_received()
    {
        Assert.Throws<ReceivedCallsException>(() =>
                _car.Received().Idle()
            );
    }

    [Test]
    public void Check_call_was_received_with_expected_argument()
    {
        _car.RevAt(Rpm);

        _car.Received().RevAt(Rpm);
    }

    [Test]
    public void Throw_when_expected_call_was_received_with_different_argument()
    {
        _car.RevAt(Rpm);

        Assert.Throws<ReceivedCallsException>(() =>
                _car.Received().RevAt(Rpm + 2)
            );
    }

    [Test]
    public void Check_that_a_call_was_not_received()
    {
        _car.RevAt(Rpm);

        _car.DidNotReceive().RevAt(Rpm + 2);
    }

    [Test]
    public void Throw_when_a_call_was_not_expected_to_be_received()
    {
        _car.RevAt(Rpm);

        Assert.Throws<ReceivedCallsException>(() => _car.DidNotReceive().RevAt(Rpm));
    }

    [Test]
    public void Check_call_received_with_any_arguments()
    {
        _car.RevAt(Rpm);

        _car.ReceivedWithAnyArgs().RevAt(Rpm + 100);
    }

    [Test]
    public void Throw_when_call_was_expected_with_any_arguments()
    {
        Assert.Throws<ReceivedCallsException>(() => _car.ReceivedWithAnyArgs().FillPetrolTankTo(10));
    }

    [Test]
    public void Check_call_was_not_received_with_any_combination_of_arguments()
    {
        _car.DidNotReceiveWithAnyArgs().FillPetrolTankTo(10);
    }

    [Test]
    public void Throw_when_call_was_not_expected_to_be_received_with_any_combination_of_arguments()
    {
        _car.RevAt(Rpm);

        Assert.Throws<ReceivedCallsException>(() => _car.DidNotReceiveWithAnyArgs().RevAt(0));
    }

    [Test]
    public void Get_all_received_calls_returns_all_calls_in_order_received()
    {
        _car.Idle();
        _car.Rev();

        var calls = _car.ReceivedCalls();
        var callNames = string.Join(",", calls.Select(x => x.GetMethodInfo().Name).ToArray());

        Assert.That(callNames, Is.EqualTo("Idle,Rev"));
    }

    [Test]
    public void Get_all_received_calls_on_delegate()
    {
        var f = Substitute.For<Func<string>>();
        f();
        f();

        var calls = f.ReceivedCalls();
        Assert.That(calls.Count(), Is.EqualTo(2));
    }

    [Test]
    public void Should_receive_call_even_when_call_is_stubbed_to_throw_an_exception()
    {
        _car.GetCapacityInLitres().Returns(x => { throw new InvalidOperationException(); });

        var exceptionThrown = false;
        try { _car.GetCapacityInLitres(); }
        catch { exceptionThrown = true; }

        _car.Received().GetCapacityInLitres();
        Assert.That(exceptionThrown, "An exception should have been thrown for this to actually test whether calls that throw exceptions are received.");
    }

    [Test]
    public void Should_receive_call_when_a_callback_for_call_throws_an_exception()
    {
        _car.When(x => x.Rev()).Do(x => { throw new InvalidOperationException(); });

        var exceptionThrown = false;
        try { _car.Rev(); }
        catch { exceptionThrown = true; }

        _car.Received().Rev();
        Assert.That(exceptionThrown, "An exception should have been thrown for this to actually test whether calls that throw exceptions are received.");
    }

    [Test]
    public void Check_when_call_was_received_repeatedly()
    {
        _car.Rev();
        _car.Rev();
        _car.Rev();

        _car.Received(3).Rev();
    }

    [Test]
    public void Check_call_was_not_received_by_making_sure_it_was_called_zero_times()
    {
        _car.Received(0).Rev();
    }

    [Test]
    public void Throw_when_expected_call_was_not_received_enough_times()
    {
        _car.Idle();
        _car.Idle();
        _car.Idle();
        _car.Idle();

        Assert.Throws<ReceivedCallsException>(() => _car.Received(5).Idle());
    }

    [Test]
    public void Throw_when_expected_call_was_received_too_many_times()
    {
        _car.Idle();
        _car.Idle();
        _car.Idle();

        Assert.Throws<ReceivedCallsException>(() => _car.Received(2).Idle());
    }

    [Test]
    public void Throw_when_expected_call_was_not_received_exactly_zero_times()
    {
        _car.Idle();
        Assert.Throws<ReceivedCallsException>(() => _car.Received(0).Idle());
    }

    [Test]
    public void Check_call_was_received_a_specifc_number_of_times_with_expected_argument()
    {
        const int differentRpm = Rpm + 2134;
        _car.RevAt(Rpm);
        _car.RevAt(differentRpm);
        _car.RevAt(Rpm);

        _car.Received(2).RevAt(Rpm);
    }

    [Test]
    public void Throw_when_expected_call_was_not_received_a_specific_number_of_times_with_expected_argument()
    {
        _car.RevAt(Rpm);
        _car.RevAt(Rpm);

        Assert.Throws<ReceivedCallsException>(() => _car.Received(2).RevAt(Rpm + 2));
    }

    [Test]
    public void Check_call_received_a_specific_number_of_times_with_any_arguments()
    {
        _car.RevAt(1);
        _car.RevAt(2);
        _car.RevAt(3);

        _car.ReceivedWithAnyArgs(3).RevAt(Rpm + 100);
    }

    [Test]
    public void Throw_when_call_was_not_received_a_specific_number_of_times_with_any_arguments()
    {
        _car.RevAt(1);
        _car.RevAt(2);
        _car.RevAt(3);
        _car.RevAt(4);

        Assert.Throws<ReceivedCallsException>(() => _car.ReceivedWithAnyArgs(2).RevAt(0));
    }

    [Test]
    public void Check_when_call_was_received_with_reference_type_params_array()
    {
        _car.StoreLuggage(Luggage);

        _car.Received().StoreLuggage(Luggage);
    }

    [Test]
    public void Check_when_call_was_received_with_value_type_params_array()
    {
        _car.RecordServiceDates(ServiceDates);

        _car.Received().RecordServiceDates(ServiceDates);
    }

    [Test]
    [TestCase(1, false)]
    [TestCase(2, true)]
    [TestCase(3, true)]
    [TestCase(4, true)]
    [TestCase(5, true)]
    [TestCase(6, false)]
    public void Check_call_was_received_within_a_range_of_times(int numberOfCalls, bool shouldPass)
    {
        for (var i = 0; i < numberOfCalls; i++)
        {
            _car.Idle();
        }

        if (shouldPass)
        {
            _car.Received(Quantity.Within(2, 5)).Idle();
        }
        else
        {
            Assert.Throws<ReceivedCallsException>(() =>
                _car.Received(Quantity.Within(2, 5)).Idle()
            );
        }
    }

    [Test]
    public void Throw_when_call_was_not_received_within_a_range_of_times()
    {
        _car.Idle();
        var ex = Assert.Throws<ReceivedCallsException>(() =>
            _car.Received(Quantity.Within(2, 5)).Idle()
        );
        Assert.That(ex.Message, Does.Contain("between 2 and 5 (inclusive) calls"));
    }

    [Test]
    public void Throw_when_call_was_received_too_many_times_within_a_range()
    {
        _car.Idle();
        _car.Idle();
        var ex = Assert.Throws<ReceivedCallsException>(() =>
            _car.Received(Quantity.Within(0, 1)).Idle()
        );
        Assert.That(ex.Message, Does.Contain("between 0 and 1 (inclusive) call"));
    }

    [Test]
    public void Throw_when_invalid_range_given()
    {
        _car.Idle();
        _car.Idle();
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            _car.Received(Quantity.Within(3, 2)).Idle()
        );
        Assert.That(ex.Message, Does.Contain("maxInclusive must be greater than minInclusive (was 2, required > 3)."));
    }

    [Test]
    public void Throw_when_negative_min_range_given()
    {
        _car.Idle();
        _car.Idle();
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            _car.Received(Quantity.Within(-1, 1)).Idle()
        );
        Assert.That(ex.Message, Does.Contain("minInclusive must be >= 0, but was -1."));
    }

    [Test]
    public void Works_with_byref_generic_parameters()
    {
        IMyService service = Substitute.For<IMyService>();
        MyArgument arg = new();
        service.MyMethod(ref arg);

        service.Received().MyMethod(ref Arg.Any<Arg.AnyType>());
    }

    public interface ICar
    {
        void Start();
        void Rev();
        void Stop();
        void Idle();
        void RevAt(int rpm);
        void FillPetrolTankTo(int percent);
        void StoreLuggage(params object[] luggage);
        void RecordServiceDates(params DateTime[] serviceDates);
        float GetCapacityInLitres();
        event Action Started;
    }

    public interface IMyService
    {
        void MyMethod<T>(ref T argument);
    }

    public class MyArgument { }
}