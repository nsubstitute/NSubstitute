using System;
using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public class SuppliedArgumentSpecificationsFactory : ISuppliedArgumentSpecificationsFactory
    {
        public ISuppliedArgumentSpecifications Create(IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            return new SuppliedArgumentSpecifications(argumentSpecifications);
        }
    }
}