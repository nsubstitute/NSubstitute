using System;
using NSubstitute.Core;

namespace NSubstitute
{
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
        public static T For<T>(params object[] constructorArguments) 
            where T : class
        {
            return (T) For(new[] {typeof(T)}, constructorArguments);
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
        public static T1 For<T1, T2>(params object[] constructorArguments)
            where T1 : class
            where T2 : class
        {
            return (T1) For(new[] { typeof(T1), typeof(T2) }, constructorArguments);
        }

        /// <summary>
        /// <para>Substitute for multiple interfaces or a class that implements multiple interfaces. At most one class can be specified.</para>
        /// If additional interfaces are required use the <see cref="M:For(System.Type[], System.Object[])" /> overload.
        /// <para>Be careful when specifying a class, as all non-virtual members will actually be executed. Only virtual members 
        /// can be recorded or have return values specified.</para>
        /// </summary>
        /// <typeparam name="T1">The type of interface or class to substitute.</typeparam>
        /// <typeparam name="T2">An additional interface or class (maximum of one class) the substitute should implement.</typeparam>
        /// <typeparam name="T3">An additional interface or class (maximum of one class) the substitute should implement.</typeparam>
        /// <param name="constructorArguments">Arguments required to construct a class being substituted. Not required for interfaces or classes with default constructors.</param>
        /// <returns>A substitute of type T1, that also implements T2 and T3.</returns>
        public static T1 For<T1, T2, T3>(params object[] constructorArguments)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return (T1) For(new[] { typeof(T1), typeof(T2), typeof(T3) }, constructorArguments);
        }

        /// <summary>
        /// <para>Substitute for multiple interfaces or a class that implements multiple interfaces. At most one class can be specified.</para>
        /// <para>Be careful when specifying a class, as all non-virtual members will actually be executed. Only virtual members 
        /// can be recorded or have return values specified.</para>
        /// </summary>
        /// <param name="typesToProxy">The types of interfaces or a type of class and multiple interfaces the substitute should implement.</param>
        /// <param name="constructorArguments">Arguments required to construct a class being substituted. Not required for interfaces or classes with default constructors.</param>
        /// <returns>A substitute implementing the specified types.</returns>
        public static object For(Type[] typesToProxy, object[] constructorArguments) 
        {
            var substituteFactory = SubstitutionContext.Current.SubstituteFactory;
            return substituteFactory.Create(typesToProxy, constructorArguments);
        }
    }
}