using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class ArgumentSpecificationDequeue : IArgumentSpecificationDequeue
    {
        private static readonly IArgumentSpecification[] EmptySpecifications = new IArgumentSpecification[0];

        private readonly Func<IList<IArgumentSpecification>> _dequeueAllQueuedArgSpecs;

        public ArgumentSpecificationDequeue(Func<IList<IArgumentSpecification>> dequeueAllQueuedArgSpecs)
        {
            _dequeueAllQueuedArgSpecs = dequeueAllQueuedArgSpecs;
        }

        public IList<IArgumentSpecification> DequeueAllArgumentSpecificationsForMethod(int parametersCount)
        {
            if (parametersCount == 0)
            {
                // We violate public contract, as mutable list was expected as result.
                // However, in reality we never expect value to be mutated, so this optimization is fine.
                // We are not allowed to change public contract due to SemVer, so keeping that as it is.
                return EmptySpecifications;
            }

            var queuedArgSpecifications = _dequeueAllQueuedArgSpecs.Invoke();
            return queuedArgSpecifications;
        }

        public IList<IArgumentSpecification> DequeueAllArgumentSpecificationsForMethod(MethodInfo methodInfo)
        {
            return DequeueAllArgumentSpecificationsForMethod(methodInfo.GetParameters().Length);
        }
    }
}