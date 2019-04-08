using System;

namespace NSubstitute.Proxies.DelegateProxy
{
    [Obsolete("This class is deprecated and will be removed in future versions of the product.")]
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