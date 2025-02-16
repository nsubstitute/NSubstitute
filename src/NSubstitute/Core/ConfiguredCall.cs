namespace NSubstitute.Core;

public class ConfiguredCall(Action<Action<CallInfo>> addAction)
{

    /// <summary>
    /// Adds a callback to execute for matching calls.
    /// </summary>
    /// <param name="action">an action to call</param>
    /// <returns></returns>
    public ConfiguredCall AndDoes(Action<CallInfo> action)
    {
        addAction(action);
        return this;
    }
}