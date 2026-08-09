using System.Reflection;

namespace NSubstitute.Core.Events;

public class DelegateEventWrapper<T>(params object?[] arguments) : RaiseEventWrapper
{
    protected override string RaiseMethodName => "Raise.Event";

    // Disable nullability for client API, so it does not affect clients.
#nullable disable annotations
    public static implicit operator T(DelegateEventWrapper<T> wrapper)
    {
        RaiseEvent(wrapper);
        return default;
    }
#nullable restore annotations

    protected override object?[] WorkOutRequiredArguments(ICall call)
    {
        var requiredArgs = typeof(T).GetInvokeMethod().GetParameters();

        if (arguments.Length < 2 && LooksLikeAnEventStyleCall(requiredArgs))
        {
            return WorkOutSenderAndEventArgs(requiredArgs[1].ParameterType, call);
        }

        if (!RequiredArgsHaveBeenProvided(arguments, requiredArgs))
        {
            ThrowBecauseRequiredArgsNotProvided(requiredArgs);
        }

        return arguments;
    }

    private bool LooksLikeAnEventStyleCall(ParameterInfo[] parameters)
    {
        return parameters.Length == 2 &&
               parameters[0].ParameterType == typeof(object) &&
               typeof(EventArgs).IsAssignableFrom(parameters[1].ParameterType);
    }

    private object?[] WorkOutSenderAndEventArgs(Type eventArgsType, ICall call)
    {
        object? sender;
        object? eventArgs;
        if (arguments.Length == 0)
        {
            sender = call.Target();
            eventArgs = GetDefaultForEventArgType(eventArgsType);
        }
        else if (arguments[0].IsCompatibleWith(eventArgsType))
        {
            sender = call.Target();
            eventArgs = arguments[0];
        }
        else
        {
            sender = arguments[0];
            eventArgs = GetDefaultForEventArgType(eventArgsType);
        }
        return [sender, eventArgs];
    }

    private static bool RequiredArgsHaveBeenProvided(object?[] providedArgs, ParameterInfo[] requiredArgs)
    {
        if (providedArgs.Length != requiredArgs.Length)
        {
            return false;
        }

        for (var i = 0; i < providedArgs.Length; i++)
        {
            var requiredArgType = requiredArgs[i].ParameterType;
            var providedArg = providedArgs[i];
            if (!providedArg.IsCompatibleWith(requiredArgType))
            {
                return false;
            }
        }

        return true;
    }

    private static void ThrowBecauseRequiredArgsNotProvided(ParameterInfo[] requiredArgs)
    {
        var message = string.Format(
            "Cannot raise event with the provided arguments. Use Raise.Event<{0}>({1}) to raise this event.",
            typeof(T).Name,
            string.Join(", ", requiredArgs.Select(x => x.ParameterType.Name).ToArray()));

        throw new ArgumentException(message);
    }
}