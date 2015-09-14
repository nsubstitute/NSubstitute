using System;
using System.Collections.Generic;
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
				this.actions = new Stack<Action<CallInfo>>(actions);
			}

			public void Call(CallInfo ci)
			{
				actions.Pop().Invoke(ci);
			}
		}
	}
}