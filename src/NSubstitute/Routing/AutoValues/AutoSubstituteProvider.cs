using System;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoSubstituteProvider : IAutoValueProvider
    {
        private readonly ISubstituteFactory _substituteFactory;

        public AutoSubstituteProvider(ISubstituteFactory substituteFactory)
        {
            _substituteFactory = substituteFactory;
        }

        public bool CanProvideValueFor(Type type)
        {
            return type.IsInterface()
                || type.IsSubclassOf(typeof(Delegate))
                || IsPureVirtualClassWithParameterlessConstructor(type);
        }

        public object GetValue(Type type)
        {
            return _substituteFactory.Create(new[] { type }, new object[0]);
        }

        private bool IsPureVirtualClassWithParameterlessConstructor(Type type)
        {
            if (type == typeof(object)) return false;
            if (!type.IsClass()) return false;
            if (!IsPureVirtualType(type)) return false;
            if (!HasParameterlessConstructor(type)) return false;
            return true;
        }

        private bool HasParameterlessConstructor(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var parameterlessConstructors = constructors.Where(x => IsCallableFromProxy(x) && x.GetParameters().Length == 0);
            if (!parameterlessConstructors.Any()) return false;
            return true;
        }

        private bool IsPureVirtualType(Type type)
        {
            if (type.IsSealed()) return false;
            var methods = type.GetMethods().Where(NotMethodFromObject).Where(NotStaticMethod);
            return methods.All(IsOverridable);
        }

        private bool IsCallableFromProxy(MethodBase constructor)
        {
            return constructor.IsPublic || constructor.IsFamily || constructor.IsFamilyOrAssembly;
        }

        private bool IsOverridable(MethodInfo methodInfo)
        {
            return methodInfo.IsVirtual && !methodInfo.IsFinal;
        }

        private bool NotMethodFromObject(MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType != typeof(object);
        }

        private bool NotStaticMethod(MethodInfo methodInfo)
        {
            return !methodInfo.IsStatic;
        }
    }
}