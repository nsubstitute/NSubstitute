using System;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public static class ReflectionExtensions
    {
        public static PropertyInfo GetPropertyFromSetterCallOrNull(this MethodInfo call)
        {
            var properties = call.DeclaringType.GetProperties();
            return properties.FirstOrDefault(x => x.GetSetMethod() == call);
        }

        public static PropertyInfo GetPropertyFromGetterCallOrNull(this MethodInfo call)
        {
            var properties = call.DeclaringType.GetProperties();
            return properties.FirstOrDefault(x => x.GetGetMethod() == call);
        }

        public static bool IsParams(this ParameterInfo parameterInfo)
        {
            return parameterInfo.IsDefined(typeof(ParamArrayAttribute), false);
        }
    }
}