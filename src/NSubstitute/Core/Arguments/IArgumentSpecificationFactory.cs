namespace NSubstitute.Core.Arguments
{
    public interface IArgumentSpecificationFactory
    {
        IArgumentSpecification Create(object? argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications);
    }
}