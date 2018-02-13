using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class ArgumentSpecificationDequeue : IArgumentSpecificationDequeue
    {
        private readonly Func<IList<IArgumentSpecification>> _dequeueAllQueuedArgSpecs;

        public ArgumentSpecificationDequeue(Func<IList<IArgumentSpecification>> dequeueAllQueuedArgSpecs)
        {
            _dequeueAllQueuedArgSpecs = dequeueAllQueuedArgSpecs;
        }
        
        public IList<IArgumentSpecification> DequeueAllArgumentSpecificationsForMethod(MethodInfo methodInfo)
        {
            if (methodInfo.GetParameters().Length == 0)
                return new List<IArgumentSpecification>();

            var queuedArgSpecifications = _dequeueAllQueuedArgSpecs.Invoke();
            return queuedArgSpecifications;
        }
    }
}