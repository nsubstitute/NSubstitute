using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object[] arguments, IParameterInfo[] parameterInfos, MethodInfo methodInfo)
        {
            var suppliedArgumentSpecifications = _suppliedArgumentSpecificationsFactory.Create(argumentSpecs);

            var result = new List<IArgumentSpecification>();
            for (var i = 0; i < arguments.Length; i++)
            {
                var arg = arguments[i];
                var paramInfo = parameterInfos[i];

                try
                {
                    result.Add(_argumentSpecificationFactory.Create(arg, paramInfo, suppliedArgumentSpecifications));
                }
                catch (AmbiguousArgumentsException ex) when (ex.ContainsDefaultMessage)
                {
                    IEnumerable<IArgumentSpecification> alreadyResolvedSpecs = result;
                    if (ex.Data[AmbiguousArgumentsException.NonReportedResolvedSpecificationsKey] is IEnumerable<IArgumentSpecification> additional)
                    {
                        alreadyResolvedSpecs = alreadyResolvedSpecs.Concat(additional);
                    }

                    throw new AmbiguousArgumentsException(methodInfo, arguments, alreadyResolvedSpecs, argumentSpecs);
                }
            }

            var remainingArgumentSpecifications = suppliedArgumentSpecifications.DequeueRemaining();
            if (remainingArgumentSpecifications.Any())
            {
                throw new RedundantArgumentMatcherException(remainingArgumentSpecifications, argumentSpecs);
            }

            return result;
        }
    }
}