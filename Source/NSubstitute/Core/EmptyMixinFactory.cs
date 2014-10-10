using System;

namespace NSubstitute.Core
{
	public class EmptyMixinFactory : IMixinFactory
	{
		public object[] Create(Type primaryProxyType, Type[] additionalTypes, ISubstitutionContext substitutionContext, ISubstituteState substituteState)
		{
			return new object[0];
		}
	}
}