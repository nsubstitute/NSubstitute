using System;

namespace NSubstitute.Proxies.DelegateProxy
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ProxiedDelegateTypeAttribute : Attribute
    {
        public Type DelegateType { get; }

        public ProxiedDelegateTypeAttribute(Type delegateType)
        {
            DelegateType = delegateType;
        }
    }
}