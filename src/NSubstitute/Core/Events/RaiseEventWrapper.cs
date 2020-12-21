using System;
using System.Reflection;
using NSubstitute.Exceptions;

namespace NSubstitute.Core.Events
{
    public abstract class RaiseEventWrapper
    {
        protected abstract object?[] WorkOutRequiredArguments(ICall call);
        protected abstract string RaiseMethodName { get; }

        protected EventArgs GetDefaultForEventArgType(Type type)
        {
            if (type == typeof(EventArgs)) return EventArgs.Empty;

            var defaultConstructor = GetDefaultConstructor(type);
            if (defaultConstructor == null)
            {
                var message = string.Format(
                    "Cannot create {0} for this event as it has no default constructor. " +
                    "Provide arguments for this event by calling {1}({0})."
                    , type.Name, RaiseMethodName);
                throw new CannotCreateEventArgsException(message);
            }
            return (EventArgs)defaultConstructor.Invoke(new object[0]);
        }

        private static ConstructorInfo? GetDefaultConstructor(Type type) => type.GetConstructor(Type.EmptyTypes);

        protected static void RaiseEvent(RaiseEventWrapper wrapper)
        {
            var context = SubstitutionContext.Current;
            context.ThreadContext.SetPendingRaisingEventArgumentsFactory(call => wrapper.WorkOutRequiredArguments(call));
        }
    }
}