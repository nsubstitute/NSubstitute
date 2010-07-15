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
            var stateProperties = typeof(SubstituteState).GetProperties();
            foreach (var property in stateProperties)
            {
                if (type.IsAssignableFrom(property.PropertyType))
                    return property.GetValue(substituteState, null);
            }
            foreach (var argument in routeArguments)
            {
                if (type.IsAssignableFrom(argument.GetType())) return argument;
            }
            throw new SubstituteException("Cannot create part. Cannot find an instance of " + type.FullName);
        }
        
        private ConstructorInfo GetConstructorFor(Type partType)
        {
            var constructors = partType.GetConstructors();
            if (constructors.Length != 1) throw new SubstituteException("Cannot create part of type " + partType.FullName + ". Make sure it only has one constructor.");
            return constructors[0];
        }
    }
}