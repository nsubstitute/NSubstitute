namespace NSubstitute.Core;

internal sealed class CustomHandlers(ISubstituteState substituteState) : ICustomHandlers
{
    private readonly List<ICallHandler> _handlers = [];

    public IReadOnlyCollection<ICallHandler> Handlers => _handlers;

    public void AddCustomHandlerFactory(CallHandlerFactory factory)
    {
        _handlers.Add(factory.Invoke(substituteState));
    }
}