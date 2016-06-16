using System;

namespace NSubstitute
{
	[Flags]
	public enum ClearanceFlags
	{
		/// <summary>
		/// Forget all the received calls
		/// </summary>
		ReceivedCalls = 1,

		/// <summary>
		/// Forget all the defined return values
		/// </summary>
		ReturnValues = 2,

		/// <summary>
		/// Forget all the define call actions
		/// </summary>
		CallActions = 4,

		/// <summary>
		/// Forget all substitutions
		/// </summary>
		All = ReceivedCalls | ReturnValues | CallActions
	}
}