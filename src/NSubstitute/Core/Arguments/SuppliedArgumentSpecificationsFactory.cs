using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public class SuppliedArgumentSpecificationsFactory : ISuppliedArgumentSpecificationsFactory
    {
        private readonly IArgumentSpecificationCompatibilityTester _argumentSpecificationCompatTester;

        public SuppliedArgumentSpecificationsFactory(IArgumentSpecificationCompatibilityTester argumentSpecificationCompatTester)
        {
            _argumentSpecificationCompatTester = argumentSpecificationCompatTester;
        }

        public ISuppliedArgumentSpecifications Create(IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            return new SuppliedArgumentSpecifications(_argumentSpecificationCompatTester, argumentSpecifications);
        }
    }
}