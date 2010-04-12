namespace NSubstitute
{
    public interface IReflectionHelper
    {
        bool IsCallToSetAReadWriteProperty(ICall call);
        ICall CreateCallToPropertyGetterFromSetterCall(ICall callToSetter);
    }
}