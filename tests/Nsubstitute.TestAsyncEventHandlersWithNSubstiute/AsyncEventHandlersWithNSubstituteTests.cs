using NSubstitute;
using NUnit.Framework;

namespace TestAsyncEventHandlersWithNSubstiute;

[TestFixture]
public class AsyncEventHandlersWithNSubstituteTests
{
    [SetUp]
    public void SetUp()
    {
        _waitFinished = false;
    }

    private bool _waitFinished;

    [Test]
    public void TestWithSynchronousHandler()
    {
        var testInterface = Substitute.For<ITestInterface>();

        testInterface.TestEvent += SynchronousEventHandler;

        testInterface.TestEvent += Raise.Event<TestEventHandler>();

        TestContext.WriteLine("Raise.Event finished");

        Assert.That(_waitFinished, Is.True);
    }

    [Test]
    public async Task TestSubstituteImplementationWithSynchronousHandler()
    {
        var testImplementation = new TestImplementation();

        testImplementation.TestEvent += SynchronousEventHandler;

        await testImplementation.RaiseEventAsync();

        TestContext.WriteLine("Raise.Event finished");

        Assert.That(_waitFinished, Is.True);
    }

    private Task SynchronousEventHandler()
    {
        TestContext.WriteLine("starting to wait");
        Thread.Sleep(100);
        _waitFinished = true;
        TestContext.WriteLine("wait finished");
        return Task.CompletedTask;
    }


    [Test]
    public void TestSubstituteWithAsynchronousHandler()
    {
        var testInterface = Substitute.For<ITestInterface>();

        testInterface.TestEvent += AsynchronousEventHandler;

        testInterface.TestEvent += Raise.Event<TestEventHandler>();

        TestContext.WriteLine("Raise.Event finished");

        Assert.That(_waitFinished, Is.True);
    }

    private async Task AsynchronousEventHandler()
    {
        TestContext.WriteLine("starting to wait");
        await Task.Delay(100);
        _waitFinished = true;
        TestContext.WriteLine("wait finished");
    }


    [Test]
    public async Task TestImplementationWithAsynchronousHandler()
    {
        var testImplementation = new TestImplementation();

        testImplementation.TestEvent += AsynchronousEventHandler;

        await testImplementation.RaiseEventAsync();

        TestContext.WriteLine("Raise.Event finished");

        Assert.That(_waitFinished, Is.True);
    }
}