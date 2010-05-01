namespace NSubstitute
{
    public interface IPropertyHelper
    {
        bool IsCallToSetAReadWriteProperty(ICall call);
        ICall CreateCallToPropertyGetterFromSetterCall(ICall callToSetter);
    }
}