using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecificationsFactory : IArgumentSpecificationsFactory
    {
        private readonly IMixedArgumentSpecificationsFactory MixedArgumentSpecificationsFactory;

        public ArgumentSpecificationsFactory(IMixedArgumentSpecificationsFactory mixedArgumentSpecificationsFactory)
        {
            MixedArgumentSpecificationsFactory = mixedArgumentSpecificationsFactory;
        }

        public IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object[] arguments, IParameterInfo[] parameterInfos, MatchArgs matchArgs)
        {
            if (matchArgs == MatchArgs.Any) return parameterInfos.Select(x => (IArgumentSpecification) new ArgumentIsAnythingSpecification(x.ParameterType));

            if (argumentSpecs.Count == arguments.Length) return argumentSpecs;

            return MixedArgumentSpecificationsFactory.Create(argumentSpecs, arguments, parameterInfos);
        }
    }
}