using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.AsyncEventHandlers;

[TestFixture]
public class AsyncEventHandlersWithNSubstituteTests
{
    #region Fields

    private bool _waitFinished;

    #endregion

    #region Setup and Teardown

    [SetUp]
    public void SetUp()
    {
        _waitFinished = false;
    }

    #endregion

    #region Public Methods

    [Test]
    public async Task TestImplementationWithAsynchronousHandler()
    {
        var testImplementation = new TestImplementation();

        testImplementation.TestEvent += AsynchronousEventHandler;

        await testImplementation.RaiseEventAsync();

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

    [Test]
    public void TestSubstituteWithAsynchronousHandler()
    {
        var testInterface = Substitute.For<ITestInterface>();

        testInterface.TestEvent += AsynchronousEventHandler;

        testInterface.TestEvent += Raise.Event<TestEventHandler>();

        TestContext.WriteLine("Raise.Event finished");

        Assert.That(_waitFinished, Is.True);
    }

    [Test]
    public void TestWithSynchronousHandler()
    {
        var testInterface = Substitute.For<ITestInterface>();

        testInterface.TestEvent += SynchronousEventHandler;

        testInterface.TestEvent += Raise.Event<TestEventHandler>();

        TestContext.WriteLine("Raise.Event finished");

        Assert.That(_waitFinished, Is.True);
    }

    #endregion

    #region Private Methods

    private async Task AsynchronousEventHandler()
    {
        TestContext.WriteLine("starting to wait");
        await Task.Delay(100);
        _waitFinished = true;
        TestContext.WriteLine("wait finished");
    }

    private Task SynchronousEventHandler()
    {
        TestContext.WriteLine("starting to wait");
        Thread.Sleep(100);
        _waitFinished = true;
        TestContext.WriteLine("wait finished");
        return Task.CompletedTask;
    }

    #endregion
}