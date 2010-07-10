using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class ArgumentSpecificationFactory : IArgumentSpecificationFactory
    {
        private readonly IMixedArgumentSpecificationFactory _mixedArgumentSpecificationFactory;

        public ArgumentSpecificationFactory(IMixedArgumentSpecificationFactory mixedArgumentSpecificationFactory)
        {
            _mixedArgumentSpecificationFactory = mixedArgumentSpecificationFactory;
        }

        public IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object[] arguments, Type[] parameterTypes, MatchArgs matchArgs)
        {
            if (matchArgs == MatchArgs.Any) return parameterTypes.Select(x => (IArgumentSpecification) new ArgumentIsAnythingSpecification(x));

            if (argumentSpecs.Count == arguments.Length) return argumentSpecs;

            return _mixedArgumentSpecificationFactory.Create(argumentSpecs, arguments, parameterTypes);
        }
    }
}