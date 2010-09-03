namespace NSubstitute.Core.Arguments
{
    public interface INonParamsArgumentSpecificationFactory
    {
        IArgumentSpecification Create(object argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications);
    }
}