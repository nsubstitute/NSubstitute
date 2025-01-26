using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;

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
    /// <exception cref="NSubstitute.Exceptions.NullSubstituteReferenceException">Substitute - Cannot mock null object</exception>
    /// <exception cref="NSubstitute.Exceptions.ProtectedMethodNotFoundException">Error mocking method.  Method must be protected virtual and with correct matching arguments and type</exception>
    /// <exception cref="System.ArgumentException">Must provide valid protected method name to mock - methodName</exception>
    public static object Protected<T>(this T obj, string methodName, params object[] args) where T : class
    {
        if (obj == null) { throw new NullSubstituteReferenceException(); }
        if (string.IsNullOrWhiteSpace(methodName)) { throw new ArgumentException("Must provide valid protected method name to mock", nameof(methodName)); }

        IList<IArgumentSpecification> argTypes = SubstitutionContext.Current.ThreadContext.PeekAllArgumentSpecifications();
        MethodInfo mthdInfo = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argTypes.Select(x => x.ForType).ToArray(), null);

        if (mthdInfo == null)
        {
            _ = SubstitutionContext.Current.ThreadContext.DequeueAllArgumentSpecifications();
            throw new ProtectedMethodNotFoundException($"No protected virtual method found with signature {methodName}({string.Join(", ", argTypes.Select(x => x.ForType))}) in {obj.GetType().BaseType!.Name}. " +
                                                    "Check that the method name and arguments are correct.  Public virtual methods must use standard NSubstitute mocking.  See the documentation for additional info.");
        }
        if (!mthdInfo.IsVirtual)
        {
            _ = SubstitutionContext.Current.ThreadContext.DequeueAllArgumentSpecifications();
            throw new ProtectedMethodNotVirtualException($"{mthdInfo} is not virtual.  NSubstitute can only work with virtual members of the class that are overridable in the test assembly");
        }

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
    /// <exception cref="NSubstitute.Exceptions.NullSubstituteReferenceException">Substitute - Cannot mock null object</exception>
    /// <exception cref="NSubstitute.Exceptions.ProtectedMethodNotFoundException">Error mocking method.  Method must be protected virtual and with correct matching arguments and type</exception>
    /// <exception cref="System.ArgumentException">Must provide valid protected method name to mock - methodName</exception>
    public static WhenCalled<T> When<T>(this T obj, string methodName, params object[] args) where T : class
    {
        if (obj == null) { throw new NullSubstituteReferenceException(); }
        if (string.IsNullOrWhiteSpace(methodName)) { throw new ArgumentException("Must provide valid protected method name to mock", nameof(methodName)); }

        IList<IArgumentSpecification> argTypes = SubstitutionContext.Current.ThreadContext.PeekAllArgumentSpecifications();
        MethodInfo mthdInfo = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argTypes.Select(y => y.ForType).ToArray(), null);

        if (mthdInfo == null)
        {
            _ = SubstitutionContext.Current.ThreadContext.DequeueAllArgumentSpecifications();
            throw new ProtectedMethodNotFoundException($"No protected virtual method found with signature {methodName}({string.Join(", ", argTypes.Select(x => x.ForType))}) in {obj.GetType().BaseType!.Name}. " +
                                                    "Check that the method name and arguments are correct.  Public virtual methods must use standard NSubstitute mocking.  See the documentation for additional info.");
        }
        if (!mthdInfo.IsVirtual)
        {
            _ = SubstitutionContext.Current.ThreadContext.DequeueAllArgumentSpecifications();
            throw new ProtectedMethodNotVirtualException($"{mthdInfo} is not virtual.  NSubstitute can only work with virtual members of the class that are overridable in the test assembly");
        }

        return new WhenCalled<T>(SubstitutionContext.Current, obj, x => mthdInfo.Invoke(x, args), MatchArgs.AsSpecifiedInCall);
    }
}