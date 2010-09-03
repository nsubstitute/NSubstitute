using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core.Arguments;

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
            var getter = propertyInfo.GetGetMethod();
            return new CallToPropertyGetter(getter, callToSetter.Target());
        }

        class CallToPropertyGetter : ICall
        {
            private readonly MethodInfo _methodInfo;
            readonly object _target;
            private readonly object[] _arguments = new object[0];
            private readonly IParameterInfo[] _parameterInfos = new IParameterInfo[0];

            public CallToPropertyGetter(MethodInfo methodInfo, object target)
            {
                _methodInfo = methodInfo;
                _target = target;
            }

            public Type GetReturnType() { return _methodInfo.ReturnType; }
            public MethodInfo GetMethodInfo() { return _methodInfo; }
            public object[] GetArguments() { return _arguments; }
            public object Target() { return _target; }
            public IParameterInfo[] GetParameterInfos() { return _parameterInfos; }
            public IList<IArgumentSpecification> GetArgumentSpecifications() { return new List<IArgumentSpecification>(); }
        }
    }
}
