namespace NSubstitute.Acceptance.Specs.AsyncEventHandlers;

public delegate Task TestEventHandler();

public interface ITestInterface
{
    #region Events

    event TestEventHandler TestEvent;

    #endregion
}