using System;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public static class ReflectionExtensions
    {
        public static PropertyInfo GetPropertyFromSetterCallOrNull(this MethodInfo call)
        {
            if(!CanBePropertySetterCall(call)) return null;

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

        private static bool CanBePropertySetterCall(MethodInfo call)
        {
            // It's safe to verify method prefix and signature as according to the ECMA-335 II.22.28:
            // 10. Any setter method for a property whose Name is xxx shall be called set_xxx [CLS]
            // 13. Any getter and setter methods shall have Method.Flags.SpecialName = 1 [CLS] 
            return call.IsSpecialName && call.Name.StartsWith("set_", StringComparison.Ordinal);
        }
    }
}