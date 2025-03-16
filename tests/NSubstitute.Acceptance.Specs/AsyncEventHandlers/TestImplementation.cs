namespace NSubstitute.Acceptance.Specs.AsyncEventHandlers;

public class TestImplementation : ITestInterface
{
    #region Events

    public event TestEventHandler? TestEvent;

    #endregion

    #region Public Methods

    public async Task RaiseEventAsync()
    {
        if (TestEvent != null)
        {
            await TestEvent();
        }
    }

    #endregion
}