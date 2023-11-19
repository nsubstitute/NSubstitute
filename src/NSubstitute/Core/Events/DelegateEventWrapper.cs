using System;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core.Events
{
    public class DelegateEventWrapper<T> : RaiseEventWrapper
    {
        private readonly object?[] _providedArguments;
        protected override string RaiseMethodName => "Raise.Event";

        public DelegateEventWrapper(params object?[] arguments)
        {
            _providedArguments = arguments;
        }

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

            if (_providedArguments.Length < 2 && LooksLikeAnEventStyleCall(requiredArgs))
            {
                return WorkOutSenderAndEventArgs(requiredArgs[1].ParameterType, call);
            }

            if (!RequiredArgsHaveBeenProvided(_providedArguments, requiredArgs))
            {
                ThrowBecauseRequiredArgsNotProvided(requiredArgs);
            }

            return _providedArguments;
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
            if (_providedArguments.Length == 0)
            {
                sender = call.Target();
                eventArgs = GetDefaultForEventArgType(eventArgsType);
            }
            else if (_providedArguments[0].IsCompatibleWith(eventArgsType))
            {
                sender = call.Target();
                eventArgs = _providedArguments[0];
            }
            else
            {
                sender = _providedArguments[0];
                eventArgs = GetDefaultForEventArgType(eventArgsType);
            }
            return new[] { sender, eventArgs };
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
}