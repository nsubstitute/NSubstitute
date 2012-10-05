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

            foreach (Expression argument in arguments)
            {
                AddConstantArgument(argument, argumentSpecifications);
                AddMethodCallArgument(argument, argumentSpecifications);
            }

            return argumentSpecifications;
        }

        private void AddMethodCallArgument(Expression argument, List<IArgumentSpecification> argumentSpecifications)
        {
            MethodCallExpression methodCall = argument as MethodCallExpression;

            if(methodCall != null)
            {
                object result = InvokeMethod(methodCall);

                var argSpecs = _context.DequeueAllArgumentSpecifications();

                if (argSpecs.Any())
                {
                    argumentSpecifications.Add(argSpecs.Single());
                }
                else
                {
                    argumentSpecifications.Add(new ArgumentSpecification(result.GetType(), new EqualsArgumentMatcher(result)));
                }
            }
        }

        private object InvokeMethod(MethodCallExpression methodCall)
        {
            object result = Expression.Lambda(methodCall).Compile().DynamicInvoke();
            
            return result;
        }

        private void AddConstantArgument(Expression argument, List<IArgumentSpecification> argumentSpecifications)
        {
            ConstantExpression constantArgument = argument as ConstantExpression;

            if(constantArgument != null)
            {
                argumentSpecifications.Add(new ArgumentSpecification(constantArgument.Type,
                                                                     new EqualsArgumentMatcher(constantArgument.Value)));
            }
        }
    }
}
