using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.Routing
{
    public class CallHandlerFactory : ICallHandlerFactory
    {
        public ICallHandler CreateCallHandler(Type handlerType, ISubstituteState substituteState, object[] routeArguments)
        {
            var constructor = GetConstructorFor(handlerType);
            var parameterTypes = constructor.GetParameters().Select(x => x.ParameterType);
            var parameters = GetParameters(parameterTypes, substituteState, routeArguments);
            return (ICallHandler) constructor.Invoke(parameters);
        }

        private object[] GetParameters(IEnumerable<Type> parameterTypes, ISubstituteState substituteState, object[] routeArguments)
        {
            return parameterTypes.Select(x => GetParameter(x, substituteState, routeArguments)).ToArray();
        }

        private object GetParameter(Type type, ISubstituteState substituteState, object[] routeArguments)
        {
            var parameter = substituteState.FindInstanceFor(type, routeArguments);
            if (parameter == null)
            {
                throw new SubstituteException("Cannot create handler. Cannot find an instance of " + type.FullName);
            }
            return parameter;
        }
        
        private ConstructorInfo GetConstructorFor(Type handlerType)
        {
            var constructors = handlerType.GetConstructors();
            if (constructors.Length != 1) throw new SubstituteException("Cannot create handler of type " + handlerType.FullName + ". Make sure it only has one constructor.");
            return constructors[0];
        }
    }
}