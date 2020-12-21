using System;

// Disable nullability for entry-point API
#nullable disable annotations

namespace NSubstitute.Core
{
    public class ConfiguredCall
    {
        private readonly Action<Action<CallInfo>> _addAction;

        public ConfiguredCall(Action<Action<CallInfo>> addAction)
        {
            _addAction = addAction;
        }

        /// <summary>
        /// Adds a callback to execute for matching calls.
        /// </summary>
        /// <param name="action">an action to call</param>
        /// <returns></returns>
        public ConfiguredCall AndDoes(Action<CallInfo> action)
        {
            _addAction(action);
            return this;
        }
    }
}