using System;
using System.Linq;
using System.Reflection;

namespace NSubstitute
{
    public class ReflectionHelper : IReflectionHelper
    {
        public bool IsCallToSetAReadWriteProperty(ICall call)
        {
            var propertySetter = GetPropertyFromSetterCallOrNull(call);
            return PropertySetterExistsAndHasAGetMethod(propertySetter);
        }

        private bool PropertySetterExistsAndHasAGetMethod(PropertyInfo propertySetter)
        {
            return propertySetter != null && propertySetter.GetGetMethod() != null;
        }

        private PropertyInfo GetPropertyFromSetterCallOrNull(ICall call)
        {
            var methodInfo = call.GetMethodInfo();
            var properties = methodInfo.DeclaringType.GetProperties();
            return properties.FirstOrDefault(x => x.GetSetMethod() == methodInfo);
        }

        public ICall CreateCallToPropertyGetterFromSetterCall(ICall callToSetter)
        {
            var propertyInfo = GetPropertyFromSetterCallOrNull(callToSetter);
            if (!PropertySetterExistsAndHasAGetMethod(propertyInfo))
            {
                throw new InvalidOperationException("Could not find a GetMethod for \"" + callToSetter.GetMethodInfo() + "\"");
            }
            var getter = propertyInfo.GetGetMethod();
            return new CallToPropertyGetter(getter);
        }

        public void RaiseEventFromEventAssignment(ICall eventAssignment, object[] arguments)
        {
            
        }

        class CallToPropertyGetter : ICall
        {
            private readonly MethodInfo _methodInfo;
            private readonly object[] _arguments = new object[0];

            public CallToPropertyGetter(MethodInfo methodInfo)
            {
                _methodInfo = methodInfo;
            }

            public Type GetReturnType() { return _methodInfo.ReturnType; }
            public MethodInfo GetMethodInfo() { return _methodInfo; }
            public object[] GetArguments() { return _arguments; }
        }
    }
}
