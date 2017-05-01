using System;

namespace NSubstitute
{
	[Flags]
	public enum ClearOptions
	{
		/// <summary>
		/// Clear all the received calls
		/// </summary>
		ReceivedCalls = 1,

		/// <summary>
		/// Clear all configured return results (including auto-substituted values).
		/// </summary>
		ReturnValues = 2,

		/// <summary>
		/// Clear all call actions configured for this substitute (via When..Do, Arg.Invoke, and Arg.Do)
		/// </summary>
		CallActions = 4,

		/// <summary>
		/// Clears all received calls and configured return values and callbacks.
		/// </summary>
		All = ReceivedCalls | ReturnValues | CallActions
	}
}