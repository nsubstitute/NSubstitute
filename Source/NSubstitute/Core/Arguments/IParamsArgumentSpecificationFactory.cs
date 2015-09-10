namespace NSubstitute.Core.Arguments
{
    public interface IParamsArgumentSpecificationFactory
    {
        IArgumentSpecification Create(object argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications);
    }
}