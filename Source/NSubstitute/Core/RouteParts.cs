using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class RouteParts : IRouteParts
    {
        private readonly SubstituteState _substituteState;
        private readonly object[] _routeArguments;

        public RouteParts(SubstituteState substituteState, object[] routeArguments)
        {
            _substituteState = substituteState;
            _routeArguments = routeArguments;
        }

        public ICallHandler GetPart<TPart>() where TPart : ICallHandler
        {
            return CreatePart(typeof (TPart));
        }

        private ICallHandler CreatePart(Type partType)
        {
            var constructor = GetConstructorFor(partType);
            var parameterTypes = constructor.GetParameters().Select(x => x.ParameterType);
            var parameters = GetParameters(parameterTypes);
            return (ICallHandler) constructor.Invoke(parameters);
        }

        private object[] GetParameters(IEnumerable<Type> parameterTypes)
        {
            return parameterTypes.Select(x => GetParameter(x)).ToArray();
        }

        private object GetParameter(Type type)
        {
            var stateProperties = typeof(SubstituteState).GetProperties();
            foreach (var property in stateProperties)
            {
                if (type.IsAssignableFrom(property.PropertyType))
                    return property.GetValue(_substituteState, null);
            }
            foreach (var argument in _routeArguments)
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