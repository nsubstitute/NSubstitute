using System;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentAction<T>
    {
        private readonly Action<object> _action;

        public ArgumentAction(Action<object> action)
        {
            _action = action;
        }

        public void RunIfTypeIsCompatible(object argument)
        {
            if (!argument.IsCompatibleWith(typeof(T))) return;
            _action(argument);
        }
    }
}