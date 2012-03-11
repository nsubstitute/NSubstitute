using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class CallSpecificationFactory : ICallSpecificationFactory
    {
        readonly IArgumentSpecificationsFactory _argumentSpecificationsFactory;
        readonly ISubstitutionContext _context;

        public CallSpecificationFactory(ISubstitutionContext substitutionContext, IArgumentSpecificationsFactory argumentSpecificationsFactory)
        {
            _context = substitutionContext;
            _argumentSpecificationsFactory = argumentSpecificationsFactory;
        }

        public ICallSpecification CreateFrom(ICall call, MatchArgs matchArgs)
        {
            var methodInfo = call.GetMethodInfo();
            var argumentSpecs = call.GetArgumentSpecifications();
            var arguments = call.GetArguments();
            var parameterInfos = call.GetParameterInfos();
            var argumentSpecificationsForCall = _argumentSpecificationsFactory.Create(argumentSpecs, arguments, parameterInfos, matchArgs);
            return new CallSpecification(methodInfo, argumentSpecificationsForCall);
        }

        public ICallSpecification CreateFrom<T>(Expression<Action<T>> call)
        {
            MethodCallExpression methodCall = (MethodCallExpression)call.Body;
            
            CallSpecification callSpecification = new CallSpecification(methodCall.Method, GetArgumentSpecifications(methodCall.Arguments));

            return callSpecification;
        }

        private IEnumerable<IArgumentSpecification> GetArgumentSpecifications(ReadOnlyCollection<Expression> arguments)
        {
            List<IArgumentSpecification> argumentSpecifications = new List<IArgumentSpecification>();

            argumentSpecifications.AddRange(GetConstantArguments(arguments));
            argumentSpecifications.AddRange(GetMethodCallArguments(arguments));

            return argumentSpecifications;
        }

        private IEnumerable<IArgumentSpecification> GetMethodCallArguments(ReadOnlyCollection<Expression> arguments)
        {
            foreach (MethodCallExpression methodCall in arguments.OfType<MethodCallExpression>())
            {
                object result = InvokeMethod(methodCall);

                var argumentSpecifications = _context.DequeueAllArgumentSpecifications();
                
                if (argumentSpecifications.Any())
                {
                    yield return argumentSpecifications.Single();
                }
                else
                {
                    yield return new ArgumentSpecification(result.GetType(), new EqualsArgumentMatcher(result));
                }
            }
        }

        private object InvokeMethod(MethodCallExpression methodCall)
        {
            object result = Expression.Lambda(methodCall).Compile().DynamicInvoke();
            
            return result;
        }

        private IEnumerable<IArgumentSpecification> GetConstantArguments(ReadOnlyCollection<Expression> arguments)
        {
            foreach (ConstantExpression constant in arguments.OfType<ConstantExpression>())
            {
                yield return new ArgumentSpecification(constant.Type, new EqualsArgumentMatcher(constant.Value));
            }
        }
    }
}
