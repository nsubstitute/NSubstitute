using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;

// Disable nullability for client API, so it does not affect clients.
#nullable disable annotations

namespace NSubstitute.Extensions;

public static class ProtectedExtensions
{
    /// <summary>
    /// Configure behavior for a protected method with return value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="args">The method arguments.</param>
    /// <returns>Result object from the method invocation.</returns>
    /// <exception cref="System.ArgumentNullException">Substitute - Cannot mock null object</exception>
    /// <exception cref="System.ArgumentException">Must provide valid protected method name to mock - methodName</exception>
    public static object Protected<T>(this T obj, string methodName, params object[] args) where T : class
    {
        if (obj == null) { throw new ArgumentNullException(nameof(obj), "Cannot mock null object"); }
        if (string.IsNullOrWhiteSpace(methodName)) { throw new ArgumentException("Must provide valid protected method name to mock", nameof(methodName)); }

        IList<IArgumentSpecification> argTypes = SubstitutionContext.Current.ThreadContext.PeekAllArgumentSpecifications();
        MethodInfo mthdInfo = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argTypes.Select(x => x.ForType).ToArray(), null);

        if (mthdInfo == null) { throw new Exception($"Method {methodName} not found"); }
        if (!mthdInfo.IsVirtual) { throw new Exception($"Method {methodName} is not virtual"); }

        return mthdInfo.Invoke(obj, args);
    }

    /// <summary>
    /// Configure behavior for a protected method with no return vlaue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="args">The method arguments.</param>
    /// <returns>WhenCalled&lt;T&gt;.</returns>
    /// <exception cref="System.ArgumentNullException">Substitute - Cannot mock null object</exception>
    /// <exception cref="System.ArgumentException">Must provide valid protected method name to mock - methodName</exception>
    public static WhenCalled<T> When<T>(this T obj, string methodName, params object[] args) where T : class
    {
        if (obj == null) { throw new ArgumentNullException(nameof(obj), "Cannot mock null object"); }
        if (string.IsNullOrWhiteSpace(methodName)) { throw new ArgumentException("Must provide valid protected method name to mock", nameof(methodName)); }

        IList<IArgumentSpecification> argTypes = SubstitutionContext.Current.ThreadContext.PeekAllArgumentSpecifications();
        MethodInfo mthdInfo = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argTypes.Select(y => y.ForType).ToArray(), null);

        if (mthdInfo == null) { throw new Exception($"Method {methodName} not found"); }
        if (!mthdInfo.IsVirtual) { throw new Exception($"Method {methodName} is not virtual"); }

        return new WhenCalled<T>(SubstitutionContext.Current, obj, x => mthdInfo.Invoke(x, args), MatchArgs.AsSpecifiedInCall);
    }
}