using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public interface ISuppliedArgumentSpecificationsFactory
    {
        ISuppliedArgumentSpecifications Create(IEnumerable<IArgumentSpecification> argumentSpecifications);
    }
}