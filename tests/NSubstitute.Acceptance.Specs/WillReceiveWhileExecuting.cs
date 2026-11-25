using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class WillReceiveWhileExecuting
{
    [Test]
    public void Simple_Call_Sequence_Succeeds()
    {
        var sub = Substitute.For<ITestInterface>();

        WillReceive
            .InOrder(() =>
            {
                sub.Method("first");
                sub.Method("second");
            })
            .WhileExecuting(() =>
            {
                sub.Method("first");
                sub.Method("second");
            });
    }

    [Test]
    public void Missing_Call_Throws_With_Descriptive_Message()
    {
        var sub = Substitute.For<ITestInterface>();

        var ex = Assert.Throws<CallSequenceNotFoundException>(() =>
            WillReceive
                .InOrder(() =>
                {
                    sub.Method("first");
                    sub.Method("second");
                })
                .WhileExecuting(() =>
                {
                    sub.Method("first");
                }));

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Contains.Substring("Call 1: Accepted!"));
        Assert.That(ex.Message, Contains.Substring("Call 2: Not received!"));
        Assert.That(ex.Message, Contains.Substring($"Expected: {nameof(ITestInterface.Method)}(\"second\")"));
    }

    [Test]
    public void Extra_Unexpected_Call_Throws_With_Descriptive_Message()
    {
        var sub = Substitute.For<ITestInterface>();

        var ex = Assert.Throws<CallSequenceNotFoundException>(() =>
            WillReceive
                .InOrder(() =>
                {
                    sub.Method("first");
                })
                .WhileExecuting(() =>
                {
                    sub.Method("first");
                    sub.Method("extra");
                }));

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Contains.Substring("Call 1: Accepted!"));
        Assert.That(ex.Message, Contains.Substring("Call 2: Unexpected!"));
        Assert.That(ex.Message, Contains.Substring($"But was: {nameof(ITestInterface.Method)}(\"extra\")"));
    }

    [Test]
    public void Wrong_Call_Order_Throws_With_Descriptive_Message()
    {
        var sub = Substitute.For<ITestInterface>();

        var ex = Assert.Throws<CallSequenceNotFoundException>(() =>
            WillReceive
                .InOrder(() =>
                {
                    sub.Method("first");
                    sub.Method("second");
                })
                .WhileExecuting(() =>
                {
                    sub.Method("second");
                    sub.Method("first");
                }));

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Contains.Substring("Call 1: Not matched!"));
        Assert.That(ex.Message, Contains.Substring($"Expected: {nameof(ITestInterface.Method)}(\"first\")"));
        Assert.That(ex.Message, Contains.Substring($"But was: {nameof(ITestInterface.Method)}(\"second\")"));
    }

    [Test]
    public void Property_Getters_Are_Ignored()
    {
        var sub = Substitute.For<ITestInterface>();

        sub.Name = "name";

        WillReceive
            .InOrder(() =>
            {
                _ = sub.Name; // getter should be ignored
                sub.Method(1);
                sub.Method(2);
            })
            .WhileExecuting(() =>
            {
                _ = sub.Name; // getter should be ignored
                sub.Method(1);
                sub.Method(2);
            });
    }

    [Test]
    public void Event_Subscriptions_Are_Handled()
    {
        var sub = Substitute.For<ITestInterface>();
        var handler = () => { };

        WillReceive
            .InOrder(() =>
            {
                sub.OnEvent += handler;
                sub.Method(1);
                sub.OnEvent -= handler;
            })
            .WhileExecuting(() =>
            {
                sub.OnEvent += handler;
                sub.Method(1);
                sub.OnEvent -= handler;
            });
    }

    [Test]
    public void Event_Subscriptions_With_Wrong_Order_Are_Handled()
    {
        var sub = Substitute.For<ITestInterface>();
        var handler = () => { };

        var ex = Assert.Throws<CallSequenceNotFoundException>(() =>
            WillReceive
                .InOrder(() =>
                {
                    sub.OnEvent += handler;
                    sub.Method(1);
                    sub.OnEvent -= handler;
                })
                .WhileExecuting(() =>
                {
                    sub.OnEvent -= handler;
                    sub.Method(1);
                    sub.OnEvent += handler;
                }));

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Contains.Substring($"Expected: {nameof(ITestInterface.OnEvent)} += Action"));
        Assert.That(ex.Message, Contains.Substring($"Expected: {nameof(ITestInterface.OnEvent)} -= Action"));
        Assert.That(ex.Message, Contains.Substring($"But was: {nameof(ITestInterface.OnEvent)} += Action"));
        Assert.That(ex.Message, Contains.Substring($"But was: {nameof(ITestInterface.OnEvent)} -= Action"));
    }

    [Test]
    public void Multiple_Substitutes_With_Correct_Order_Succeeds()
    {
        var sub1 = Substitute.For<ITestInterface>();
        var sub2 = Substitute.For<ITestInterface>();

        WillReceive
            .InOrder(() =>
            {
                sub1.Method(1);
                sub2.Method(2);
            })
            .WhileExecuting(() =>
            {
                sub1.Method(1);
                sub2.Method(2);
            });
    }

    [Test]
    public void Multiple_Substitutes_With_Wrong_Order_Shows_Instance_Numbers()
    {
        var sub1 = Substitute.For<ITestInterface>();
        var sub2 = Substitute.For<ITestInterface>();

        var ex = Assert.Throws<CallSequenceNotFoundException>(() =>
            WillReceive
                .InOrder(() =>
                {
                    sub1.Method(1);
                    sub2.Method(2);
                })
                .WhileExecuting(() =>
                {
                    sub2.Method(2);
                    sub1.Method(1);
                }));

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Contains.Substring($"1@{nameof(WillReceiveWhileExecuting)}+{nameof(ITestInterface)}"));
        Assert.That(ex.Message, Contains.Substring($"2@{nameof(WillReceiveWhileExecuting)}+{nameof(ITestInterface)}"));
    }

    [Test]
    public void Mutable_Object_State_Is_Captured_Correctly()
    {
        var sub = Substitute.For<ITestInterface>();
        var obj = new TestObject
        {
            Value = "initial"
        };

        WillReceive
            .InOrder(() =>
            {
                sub.ComplexMethod(Arg.Is<TestObject>(x => x.Value == "initial"));
                sub.ComplexMethod(Arg.Is<TestObject>(x => x.Value == "modified"));
            })
            .WhileExecuting(() =>
            {
                sub.ComplexMethod(obj);
                obj.Value = "modified";
                sub.ComplexMethod(obj);
                obj.Value = "final"; // This change should not affect verification
            });
    }

    [Test]
    public void Collection_Modifications_Are_Captured()
    {
        var sub = Substitute.For<ITestInterface>();
        var obj = new TestObject
        {
            Numbers = new List<int> { 1, 2 }
        };

        WillReceive
            .InOrder(() =>
            {
                sub.ComplexMethod(Arg.Is<TestObject>(x => x.Numbers.Count == 2));
                sub.ComplexMethod(Arg.Is<TestObject>(x => x.Numbers.Count == 3));
            })
            .WhileExecuting(() =>
            {
                sub.ComplexMethod(obj);
                obj.Numbers.Add(3);
                sub.ComplexMethod(obj);
                obj.Numbers.Add(4); // Should not affect verification
            });
    }

    [Test]
    public void Null_Arguments_Are_Handled()
    {
        var sub = Substitute.For<ITestInterface>();

        WillReceive
            .InOrder(() =>
            {
                sub.ComplexMethod(null!);
                sub.ComplexMethod(Arg.Any<TestObject>());
            })
            .WhileExecuting(() =>
            {
                sub.ComplexMethod(null!);
                sub.ComplexMethod(new TestObject());
            });
    }

    [Test]
    public void Argument_Matchers_Are_Respected()
    {
        var sub = Substitute.For<ITestInterface>();

        WillReceive
            .InOrder(() =>
            {
                sub.Method(Arg.Is<string>(s => s.StartsWith("h")));
                sub.Method(Arg.Any<string>());
            })
            .WhileExecuting(() =>
            {
                sub.Method("hello");
                sub.Method("whatever");
            });
    }

    [Test]
    public void Non_Matching_Argument_Shows_Detailed_Error()
    {
        var sub = Substitute.For<ITestInterface>();

        var ex = Assert.Throws<CallSequenceNotFoundException>(() =>
            WillReceive
                .InOrder(() =>
                {
                    sub.Method(Arg.Is<string>(s => s.StartsWith("h")));
                })
                .WhileExecuting(() =>
                {
                    sub.Method("goodbye");
                }));

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Contains.Substring("arg[0] not matched"));
    }

    [Test]
    public void Out_Parameters_Are_Handled_Correctly()
    {
        var lookup = Substitute.For<ITestInterface>();

        lookup.TryGet("key", out _).Returns(x => { x[1] = "first"; return true; });
        lookup.TryGet("other", out _).Returns(x => { x[1] = "second"; return true; });

        WillReceive
            .InOrder(() =>
            {
                lookup.TryGet("key", out _);
                lookup.TryGet("other", out _);
            })
            .WhileExecuting(() =>
            {
                lookup.TryGet("key", out _);
                lookup.TryGet("other", out _);
            });
    }

    [Test]
    public void Ref_Parameters_State_Is_Captured()
    {
        var lookup = Substitute.For<ITestInterface>();
        var refValue = 42;

        lookup.When(x => x.ModifyRef(ref refValue)).Do(x => x[0] = 100);

        WillReceive
            .InOrder(() =>
            {
                lookup.ModifyRef(ref refValue);
                refValue = 100;
                lookup.ModifyRef(ref refValue);
            })
            .WhileExecuting(() =>
            {
                refValue = 42;
                lookup.ModifyRef(ref refValue);
                refValue = 100;
                lookup.ModifyRef(ref refValue);
                refValue = 200;
            });
    }

    public interface ITestInterface
    {
        event Action OnEvent;
        void Method(string value);
        void Method(int value);
        string Name { get; set; }
        void ComplexMethod(TestObject obj);
        bool TryGet(string key, out string value);
        void ModifyRef(ref int value);
    }

    public class TestObject
    {
        public string Value { get; set; }
        public List<int> Numbers { get; set; } = [];
    }
}