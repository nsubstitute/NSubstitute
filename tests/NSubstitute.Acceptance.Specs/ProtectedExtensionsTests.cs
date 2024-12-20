using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class ProtectedExtensionsTests
{
    [Test]
    public void Should_mock_and_verify_protected_method_with_no_args()
    {
        var expectedMsg = "unit test message";
        var sub = Substitute.For<AnotherClass>();
        var worker = new Worker();

        sub.Protected("ProtectedMethod").Returns(expectedMsg);

        Assert.That(worker.DoWork(sub), Is.EqualTo(expectedMsg));
        sub.Received(1).Protected("ProtectedMethod");
    }

    [Test]
    public void Should_mock_and_verify_protected_method_with_arg()
    {
        var expectedMsg = "unit test message";
        var sub = Substitute.For<AnotherClass>();
        var worker = new Worker();

        sub.Protected("ProtectedMethod", Arg.Any<int>()).Returns(expectedMsg);

        Assert.That(worker.DoMoreWork(sub, 5), Is.EqualTo(expectedMsg));
        var a = sub.Received(1);
        a.Protected("ProtectedMethod", Arg.Any<int>());
    }

    [Test]
    public void Should_mock_and_verify_protected_method_with_multiple_args()
    {
        var expectedMsg = "unit test message";
        var sub = Substitute.For<AnotherClass>();
        var worker = new Worker();

        sub.Protected("ProtectedMethod", Arg.Any<string>(), Arg.Any<int>(), Arg.Any<char>()).Returns(expectedMsg);

        Assert.That(worker.DoEvenMoreWork(sub, 3, 'x'), Is.EqualTo(expectedMsg));
        sub.Received(1).Protected("ProtectedMethod", Arg.Any<string>(), Arg.Any<int>(), Arg.Any<char>());
    }

    [Test]
    public void Should_mock_and_verify_method_with_no_return_and_no_args()
    {
        var count = 0;
        var sub = Substitute.For<AnotherClass>();
        var worker = new Worker();

        sub.When("ProtectedMethodWithNoReturn").Do(x => count++);

        worker.DoVoidWork(sub);
        Assert.That(count, Is.EqualTo(1));
        sub.Received(1).Protected("ProtectedMethodWithNoReturn");
    }

    [Test]
    public void Should_mock_and_verify_method_with_no_return_with_arg()
    {
        var count = 0;
        var sub = Substitute.For<AnotherClass>();
        var worker = new Worker();

        sub.When("ProtectedMethodWithNoReturn", Arg.Any<int>()).Do(x => count++);

        worker.DoVoidWork(sub, 5);
        Assert.That(count, Is.EqualTo(1));
        sub.Received(1).Protected("ProtectedMethodWithNoReturn", Arg.Any<int>());
    }

    [Test]
    public void Should_mock_and_verify_method_with_no_return_with_multiple_args()
    {
        var count = 0;
        var sub = Substitute.For<AnotherClass>();
        var worker = new Worker();

        sub.When("ProtectedMethodWithNoReturn", Arg.Any<string>(), Arg.Any<int>(), Arg.Any<char>()).Do(x => count++);

        worker.DoVoidWork(sub, 5, 'x');
        Assert.That(count, Is.EqualTo(1));
        sub.Received(1).Protected("ProtectedMethodWithNoReturn", Arg.Any<string>(), Arg.Any<int>(), Arg.Any<char>());
    }

    private class Worker
    {
        internal string DoWork(AnotherClass worker)
        {
            return worker.DoWork();
        }

        internal string DoMoreWork(AnotherClass worker, int i)
        {
            return worker.DoWork(i);
        }

        internal string DoEvenMoreWork(AnotherClass worker, int i, char j)
        {
            return worker.DoWork("worker", i, j);
        }

        internal void DoVoidWork(AnotherClass worker)
        {
            worker.DoVoidWork();
        }

        internal void DoVoidWork(AnotherClass worker, int i)
        {
            worker.DoVoidWork(i);
        }

        internal void DoVoidWork(AnotherClass worker, int i, char j)
        {
            worker.DoVoidWork("void worker", i, j);
        }
    }
}