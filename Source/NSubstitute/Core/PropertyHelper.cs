using System;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class PropertyHelper : IPropertyHelper
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
            return call.GetMethodInfo().GetPropertyFromSetterCallOrNull();
        }

        public ICall CreateCallToPropertyGetterFromSetterCall(ICall callToSetter)
        {
            var propertyInfo = GetPropertyFromSetterCallOrNull(callToSetter);
            if (!PropertySetterExistsAndHasAGetMethod(propertyInfo))
            {
                throw new InvalidOperationException("Could not find a GetMethod for \"" + callToSetter.GetMethodInfo() + "\"");
            }
            var setterArgs = callToSetter.GetArguments();
            var getter = propertyInfo.GetGetMethod();
            var getterArgs = setterArgs.Take(setterArgs.Length - 1).ToArray();
            return new Call(getter, getterArgs, callToSetter.Target(), callToSetter.GetArgumentSpecifications());
        }
    }
}
