using NSubstitute.Core;
using System.Diagnostics.Contracts;

namespace NSubstitute;

/// <summary>
/// Create a substitute for one or more types. For example: <c>Substitute.For&lt;ISomeType&gt;()</c>
/// </summary>
public static class Substitute
{
    /// <summary>
    /// Substitute for an interface or class.
    /// <para>Be careful when specifying a class, as all non-virtual members will actually be executed. Only virtual members
    /// can be recorded or have return values specified.</para>
    /// </summary>
    /// <typeparam name="T">The type of interface or class to substitute.</typeparam>
    /// <param name="constructorArguments">Arguments required to construct a class being substituted. Not required for interfaces or classes with default constructors.</param>
    /// <returns>A substitute for the interface or class.</returns>
    [Pure]
    public static T For<T>(params object[] constructorArguments)
        where T : class
    {
        return (T)For([typeof(T)], constructorArguments);
    }

    /// <summary>
    /// <para>Substitute for multiple interfaces or a class that implements an interface. At most one class can be specified.</para>
    /// <para>Be careful when specifying a class, as all non-virtual members will actually be executed. Only virtual members
    /// can be recorded or have return values specified.</para>
    /// </summary>
    /// <typeparam name="T1">The type of interface or class to substitute.</typeparam>
    /// <typeparam name="T2">An additional interface or class (maximum of one class) the substitute should implement.</typeparam>
    /// <param name="constructorArguments">Arguments required to construct a class being substituted. Not required for interfaces or classes with default constructors.</param>
    /// <returns>A substitute of type T1, that also implements T2.</returns>
    [Pure]
    public static T1 For<T1, T2>(params object[] constructorArguments)
        where T1 : class
        where T2 : class
    {
        return (T1)For([typeof(T1), typeof(T2)], constructorArguments);
    }

    /// <summary>
    /// <para>Substitute for multiple interfaces or a class that implements multiple interfaces. At most one class can be specified.</para>
    /// If additional interfaces are required use the <see cref="For(System.Type[], System.Object[])" /> overload.
    /// <para>Be careful when specifying a class, as all non-virtual members will actually be executed. Only virtual members
    /// can be recorded or have return values specified.</para>
    /// </summary>
    /// <typeparam name="T1">The type of interface or class to substitute.</typeparam>
    /// <typeparam name="T2">An additional interface or class (maximum of one class) the substitute should implement.</typeparam>
    /// <typeparam name="T3">An additional interface or class (maximum of one class) the substitute should implement.</typeparam>
    /// <param name="constructorArguments">Arguments required to construct a class being substituted. Not required for interfaces or classes with default constructors.</param>
    /// <returns>A substitute of type T1, that also implements T2 and T3.</returns>
    [Pure]
    public static T1 For<T1, T2, T3>(params object[] constructorArguments)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        return (T1)For([typeof(T1), typeof(T2), typeof(T3)], constructorArguments);
    }

    /// <summary>
    /// <para>Substitute for multiple interfaces or a class that implements multiple interfaces. At most one class can be specified.</para>
    /// <para>Be careful when specifying a class, as all non-virtual members will actually be executed. Only virtual members
    /// can be recorded or have return values specified.</para>
    /// </summary>
    /// <param name="typesToProxy">The types of interfaces or a type of class and multiple interfaces the substitute should implement.</param>
    /// <param name="constructorArguments">Arguments required to construct a class being substituted. Not required for interfaces or classes with default constructors.</param>
    /// <returns>A substitute implementing the specified types.</returns>
    [Pure]
    public static object For(Type[] typesToProxy, object[] constructorArguments)
    {
        var substituteFactory = SubstitutionContext.Current.SubstituteFactory;
        return substituteFactory.Create(typesToProxy, constructorArguments);
    }

    /// <summary>
    /// Create a substitute for a class that behaves just like a real instance of the class, but also
    /// records calls made to its virtual members and allows for specific members to be substituted
    /// by using <see cref="WhenCalled{T}.DoNotCallBase()">When(() => call).DoNotCallBase()</see> or by
    /// <see cref="SubstituteExtensions.Returns{T}(T,T,T[])">setting a value to return value</see> for that member.
    /// </summary>
    /// <typeparam name="T">The type to substitute for parts of. Must be a class; not a delegate or interface.</typeparam>
    /// <param name="constructorArguments"></param>
    /// <returns>An instance of the class that will execute real methods when called, but allows parts to be selectively
    /// overridden via `Returns` and `When..DoNotCallBase`.</returns>
    [Pure]
    public static T ForPartsOf<T>(params object[] constructorArguments)
        where T : class
    {
        var substituteFactory = SubstitutionContext.Current.SubstituteFactory;
        return (T)substituteFactory.CreatePartial([typeof(T)], constructorArguments);
    }

    /// <summary>
    /// Creates a proxy for a class that implements an interface, forwarding methods and properties to an instance of the class, effectively mimicking a real instance.
    /// Both the interface and the class must be provided as parameters.
    /// The proxy will log calls made to the interface members and delegate them to an instance of the class. Specific members can be substituted
    /// by using <see cref="WhenCalled{T}.DoNotCallBase()">When(() => call).DoNotCallBase()</see> or by
    /// <see cref="SubstituteExtensions.Returns{T}(T,T,T[])">setting a value to return value</see> for that member.
    /// This extension supports sealed classes and non-virtual members, with some limitations. Since the substituted method is non-virtual, internal calls within the object will invoke the original implementation and will not be logged.
    /// </summary>
    /// <typeparam name="TInterface">The interface the substitute will implement.</typeparam>
    /// <typeparam name="TClass">The class type implementing the interface. Must be a class; not a delegate or interface. </typeparam>
    /// <param name="constructorArguments"></param>
    /// <returns>An object implementing the selected interface. Calls will be forwarded to the actuall methods, but allows parts to be selectively
    /// overridden via `Returns` and `When..DoNotCallBase`.</returns>
    [Pure]
    public static TInterface ForTypeForwardingTo<TInterface, TClass>(params object[] constructorArguments)
        where TInterface : class
    {
        var substituteFactory = SubstitutionContext.Current.SubstituteFactory;
        return (TInterface)substituteFactory.CreatePartial([typeof(TInterface), typeof(TClass)], constructorArguments);
    }
}