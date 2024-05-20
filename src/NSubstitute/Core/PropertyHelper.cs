using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;

namespace NSubstitute.Core;

public class PropertyHelper(ICallFactory callFactory, IArgumentSpecificationCompatibilityTester argSpecCompatTester) : IPropertyHelper
{
    public bool IsCallToSetAReadWriteProperty(ICall call)
    {
        var propertySetter = GetPropertyFromSetterCall(call);
        return PropertySetterExistsAndHasAGetMethod(propertySetter);
    }

    private bool PropertySetterExistsAndHasAGetMethod([NotNullWhen(true)] PropertyInfo? propertySetter)
    {
        return propertySetter != null && propertySetter.GetGetMethod(nonPublic: true) != null;
    }

    private PropertyInfo? GetPropertyFromSetterCall(ICall call)
    {
        return call.GetMethodInfo().GetPropertyFromSetterCallOrNull();
    }

    public ICall CreateCallToPropertyGetterFromSetterCall(ICall callToSetter)
    {
        var propertyInfo = GetPropertyFromSetterCall(callToSetter);
        if (!PropertySetterExistsAndHasAGetMethod(propertyInfo))
        {
            throw new InvalidOperationException("Could not find a GetMethod for \"" + callToSetter.GetMethodInfo() + "\"");
        }

        var getter = propertyInfo.GetGetMethod(nonPublic: true);
        if (getter is null) throw new SubstituteInternalException("A property with a getter expected.");

        var getterArgs = SkipLast(callToSetter.GetOriginalArguments());
        var getterArgumentSpecifications = GetGetterCallSpecificationsFromSetterCall(callToSetter);

        return callFactory.Create(getter, getterArgs, callToSetter.Target(), getterArgumentSpecifications);
    }

    private IList<IArgumentSpecification> GetGetterCallSpecificationsFromSetterCall(ICall callToSetter)
    {
        var lastSetterArg = callToSetter.GetOriginalArguments().Last();
        var lastSetterArgType = callToSetter.GetParameterInfos().Last().ParameterType;

        var argumentSpecifications = callToSetter.GetArgumentSpecifications();
        if (argumentSpecifications.Count == 0)
            return argumentSpecifications;

        // Getter call has one less argument than the setter call (the last arg is trimmed).
        // Therefore, we need to remove the last argument specification if it's for the trimmed arg.
        // Otherwise, NSubstitute might find that the redundant argument specification is present and the
        // validation logic might trigger an exception.
        if (argSpecCompatTester.IsSpecificationCompatible(argumentSpecifications.Last(), lastSetterArg, lastSetterArgType))
        {
            argumentSpecifications = SkipLast(argumentSpecifications);
        }

        return argumentSpecifications;
    }

    private static T[] SkipLast<T>(ICollection<T> collection)
    {
        return collection.Take(collection.Count - 1).ToArray();
    }
}
