using System;
using NSubstitute.Core.DependencyInjection;

namespace NSubstitute.Core
{
    public static class CallSpecificationFactoryFactoryYesThatsRight
    {
        [Obsolete("This factory is deprecated and will be removed in future versions of the product. " +
                  "Please use '" + nameof(SubstitutionContext) + "." + nameof(SubstitutionContext.Current) + "." +
                  nameof(SubstitutionContext.Current.CallSpecificationFactory) + "' instead. " +
                  "Use " + nameof(NSubstituteDefaultFactory) + " services if you need to activate a new instance.")]
        // ReSharper disable once UnusedMember.Global - is left for API compatibility from other libraries.
        public static ICallSpecificationFactory CreateCallSpecFactory() =>
            NSubstituteDefaultFactory.DefaultContainer.Resolve<ICallSpecificationFactory>();
    }
}