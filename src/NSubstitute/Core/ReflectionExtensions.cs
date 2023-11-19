using System;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public static class ReflectionExtensions
    {
        public static PropertyInfo? GetPropertyFromSetterCallOrNull(this MethodInfo call)
        {
            if (!CanBePropertySetterCall(call)) return null;

            // Don't use .FirstOrDefault() lambda, as closure leads to allocation even if not reached.
            foreach (var property in GetAllProperties(call.DeclaringType))
            {
                if (property.GetSetMethod(nonPublic: true) == call) return property;
            }

            return null;
        }

        public static PropertyInfo? GetPropertyFromGetterCallOrNull(this MethodInfo call)
        {
            return GetAllProperties(call.DeclaringType)
                .FirstOrDefault(x => x.GetGetMethod(nonPublic: true) == call);
        }

        public static bool IsParams(this ParameterInfo parameterInfo)
        {
            return parameterInfo.IsDefined(typeof(ParamArrayAttribute), inherit: false);
        }

        private static bool CanBePropertySetterCall(MethodInfo call)
        {
            // It's safe to verify method prefix and signature as according to the ECMA-335 II.22.28:
            // 10. Any setter method for a property whose Name is xxx shall be called set_xxx [CLS]
            // 13. Any getter and setter methods shall have Method.Flags.SpecialName = 1 [CLS] 
            // Notice, even though it's correct to check the SpecialName flag, we don't do that deliberately.
            // The reason is that some compilers (e.g. F#) might not emit this attribute and our library
            // misbehaves in those cases. We use slightly slower, but robust check.
            return call.Name.StartsWith("set_", StringComparison.Ordinal);
        }

        private static PropertyInfo[] GetAllProperties(Type? type)
        {
            return type != null
                ? type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                : new PropertyInfo[0];
        }
    }
}
