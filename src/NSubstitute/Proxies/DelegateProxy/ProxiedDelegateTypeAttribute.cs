namespace NSubstitute.Proxies.DelegateProxy;

[Obsolete("This class is deprecated and will be removed in future versions of the product.")]
[AttributeUsage(AttributeTargets.Method)]
public class ProxiedDelegateTypeAttribute(Type delegateType) : Attribute
{
    public Type DelegateType { get; } = delegateType;
}