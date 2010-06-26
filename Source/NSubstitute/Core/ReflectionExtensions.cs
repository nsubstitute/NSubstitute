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
    }
}