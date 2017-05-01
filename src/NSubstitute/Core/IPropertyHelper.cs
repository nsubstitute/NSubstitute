namespace NSubstitute.Core
{
    public interface IPropertyHelper
    {
        bool IsCallToSetAReadWriteProperty(ICall call);
        ICall CreateCallToPropertyGetterFromSetterCall(ICall callToSetter);
    }
}