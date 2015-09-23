using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Extensions
{
    public static class ConsecutiveDoExtension
    {
        public static void Do<T>(this WhenCalled<T> whenCalled, params Action<CallInfo>[] calls)
        {
            var cc = new ConsecutiveCall(calls);
            whenCalled.Do(cc.Call);
        }

        private class ConsecutiveCall
        {
            private readonly Stack<Action<CallInfo>> actions;

            public ConsecutiveCall(params Action<CallInfo>[] actions)
            {
                this.actions = new Stack<Action<CallInfo>>();
                for (int i = actions.Length; i > 0; i--)
                    this.actions.Push(actions[i - 1]);
            }

            public void Call(CallInfo ci)
            {
                if (actions.Any())
                    actions.Pop().Invoke(ci);
            }
        }
    }
}