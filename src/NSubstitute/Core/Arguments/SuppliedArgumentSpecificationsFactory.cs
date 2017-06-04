using System;
using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public class SuppliedArgumentSpecificationsFactory : ISuppliedArgumentSpecificationsFactory
    {
        private readonly IDefaultChecker _defaultChecker;

        public SuppliedArgumentSpecificationsFactory(IDefaultChecker defaultChecker)
        {
            _defaultChecker = defaultChecker;
        }

        public ISuppliedArgumentSpecifications Create(IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            return new SuppliedArgumentSpecifications(_defaultChecker, argumentSpecifications);
        }
    }
}