using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecificationsFactory : IArgumentSpecificationsFactory
    {
        private readonly IMixedArgumentSpecificationsFactory _mixedArgumentSpecificationsFactory;

        public ArgumentSpecificationsFactory(IMixedArgumentSpecificationsFactory mixedArgumentSpecificationsFactory)
        {
            _mixedArgumentSpecificationsFactory = mixedArgumentSpecificationsFactory;
        }

        public IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs,
            object[] arguments, IParameterInfo[] parameterInfos, MethodInfo methodInfo, MatchArgs matchArgs)
        {
            var argumentSpecifications = _mixedArgumentSpecificationsFactory.Create(argumentSpecs, arguments, parameterInfos, methodInfo);

            return (matchArgs == MatchArgs.Any) 
                ? argumentSpecifications.Select(x => x.CreateCopyMatchingAnyArgOfType(x.ForType))
                : argumentSpecifications;
        }
    }
}