using System;

namespace NSubstitute.Core
{
	public interface IMixinFactory
	{
		object[] Create(Type primaryProxyType, Type[] additionalTypes, ISubstitutionContext substitutionContext, ISubstituteState substituteState);
	}
}