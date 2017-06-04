using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class MixedArgumentSpecificationsFactory : IMixedArgumentSpecificationsFactory
    {
        private readonly IArgumentSpecificationFactory _argumentSpecificationFactory;
        private readonly ISuppliedArgumentSpecificationsFactory _suppliedArgumentSpecificationsFactory;

        public MixedArgumentSpecificationsFactory(IArgumentSpecificationFactory argumentSpecificationFactory, ISuppliedArgumentSpecificationsFactory suppliedArgumentSpecificationsFactory)
        {
            _argumentSpecificationFactory = argumentSpecificationFactory;
            _suppliedArgumentSpecificationsFactory = suppliedArgumentSpecificationsFactory;
        }

        public IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object[] arguments, IParameterInfo[] parameterInfos)
        {
            var suppliedArgumentSpecifications = _suppliedArgumentSpecificationsFactory.Create(argumentSpecs);
            return 
                new List<IArgumentSpecification>(
                    arguments.Select(
                        (argument, i) => _argumentSpecificationFactory.Create(
                            argument, parameterInfos[i], suppliedArgumentSpecifications)
                    )
                );
        }
    }
}