using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class CallFactory
    {
        private readonly ISubstitutionContext _context;

        public CallFactory() : this(SubstitutionContext.Current) { }

        public CallFactory(ISubstitutionContext context)
        {
            _context = context;
        }

        public ICall Create(MethodInfo methodInfo, object[] arguments, object target, Func<object> baseMethod)
        {
            var argSpecs = (methodInfo.GetParameters().Length == 0) ? EmptyList() : _context.DequeueAllArgumentSpecifications();
            return new Call(methodInfo, arguments, target, argSpecs, baseMethod);
        }

        private IList<IArgumentSpecification> EmptyList()
        {
            return new List<IArgumentSpecification>();
        }
    }
}