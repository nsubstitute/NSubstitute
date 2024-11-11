namespace TestAsyncEventHandlersWithNSubstiute
{

    public class TestImplementation : ITestInterface
    {
        public event TestEventHandler? TestEvent;

        public async Task RaiseEventAsync()
        {
            if (TestEvent != null) await TestEvent();
        }
    }
}
