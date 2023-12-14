namespace TestAsyncEventHandlersWithNSubstiute;

public delegate Task TestEventHandler();

public interface ITestInterface
{
    event TestEventHandler TestEvent;
}