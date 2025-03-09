namespace NSubstitute.Core.Arguments;

internal sealed class SuppliedArgumentSpecificationsFactory(IArgumentSpecificationCompatibilityTester argumentSpecificationCompatTester) : ISuppliedArgumentSpecificationsFactory
{
    public ISuppliedArgumentSpecifications Create(IEnumerable<IArgumentSpecification> argumentSpecifications)
    {
        return new SuppliedArgumentSpecifications(argumentSpecificationCompatTester, argumentSpecifications);
    }
}