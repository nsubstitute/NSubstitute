using System.Collections.Generic;
using System.Linq;
using NSubstitute.Exceptions;

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

            var result = arguments
                .Select((arg, i) => _argumentSpecificationFactory.Create(arg, parameterInfos[i], suppliedArgumentSpecifications))
                .ToArray();

            var remainingArgumentSpecifications = suppliedArgumentSpecifications.DequeueRemaining();
            if (remainingArgumentSpecifications.Any())
            {
                throw new RedundantArgumentMatcherException(remainingArgumentSpecifications, suppliedArgumentSpecifications.AllSpecifications);
            }

            return result;
        }
    }
}