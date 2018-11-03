using System;
using System.Linq.Expressions;

namespace NSubstitute.Compatibility
{
    /// <summary>
    /// Alternate version of <see cref="Arg"/> matchers for compatibility with pre-C#7 compilers
    /// which do not support <c>ref</c> return types. Do not use unless you are unable to use <see cref="Arg"/>.
    /// 
    /// <see cref="CompatArg"/> provides a non-static version of <see cref="Arg.Compat"/> which can make it easier
    /// to use from an abstract base class.
    ///
    /// For more information see <see href="http://nsubstitute.github.io/help/compat-args">Compatibility Argument
    /// Matchers</see> in the NSubstitute documentation.
    /// </summary>
    ///
    /// <example>
    /// Migrating from a pre-4.0 version of NSubstitute can result in compilation errors using argument
    /// matchers like <c>Arg.Is(42)</c> in projects using a pre-C#7 compiler. One option is to replace
    /// calls to <see cref="Arg"/> with calls to <see cref="Arg.Compat"/>. Another is to declare a field
    /// called <c>Arg</c> of type <see cref="CompatArg"/> in test fixtures. References to <c>Arg</c> will
    /// then use the compatibility version instead. This works particularly well if using a base test fixture,
    /// so all tests in the project can use the field.
    ///
    /// <code>
    /// public SampleFixture {
    ///     // Create an instance called Arg within our test fixture (or in a base test fixture if we use one).
    ///     NSubstitute.Compatibility.CompatArg Arg = new CompatArg();
    ///     
    ///     [Test]
    ///     public void SampleTest() {
    ///         var server = Substitute.For&lt;IServer&gt;();
    ///         // Using Arg in our fixture will find our instance rather than normal NSubstitute.Arg.
    ///         server.GetProductsInCategory(Arg.Is(42)).Returns(new [] { "tea", "coffee", "cocoa" });
    ///
    ///         var subject = new Lookup(server);
    ///         var result = subject.ProductCount(42);
    /// 
    ///         Assert.AreEqual(3, result);
    ///     }
    /// }
    /// </code>
    /// </example>
    public class CompatArg
    {
        /// <summary>
        /// Match any argument value compatible with type <typeparamref name="T"/>.
        /// This is provided for compatibility with older compilers --
        /// if possible use <see cref="Arg.Any{T}" /> instead.
        /// </summary>
        public T Any<T>() => Arg.Any<T>();

        /// <summary>
        /// Match argument that is equal to <paramref name="value"/>.
        /// This is provided for compatibility with older compilers --
        /// if possible use <see cref="Arg.Is{T}(T)" /> instead.
        /// </summary>
        public T Is<T>(T value) => Arg.Is(value);

        /// <summary>
        /// Match argument that satisfies <paramref name="predicate"/>. 
        /// If the <paramref name="predicate"/> throws an exception for an argument it will be treated as non-matching.
        /// This is provided for compatibility with older compilers --
        /// if possible use <see cref="Arg.Is{T}(Expression{Predicate{T}})" /> instead.
        /// </summary>
        public T Is<T>(Expression<Predicate<T>> predicate) => Arg.Is(predicate);

        /// <summary>
        /// Invoke any <see cref="Action"/> argument whenever a matching call is made to the substitute.
        /// This is provided for compatibility with older compilers --
        /// if possible use <see cref="Arg.Invoke" /> instead.
        /// </summary>
        public Action Invoke() => Arg.Invoke();

        /// <summary>
        /// Invoke any <see cref="Action&lt;T&gt;"/> argument with specified argument whenever a matching call is made to the substitute.
        /// This is provided for compatibility with older compilers --
        /// if possible use <see cref="Arg.Invoke{T}" /> instead.
        /// </summary>
        public Action<T> Invoke<T>(T arg) => Arg.Invoke(arg);

        /// <summary>
        /// Invoke any <see cref="Action&lt;T1,T2&gt;"/> argument with specified arguments whenever a matching call is made to the substitute.
        /// This is provided for compatibility with older compilers --
        /// if possible use <see cref="Arg.Invoke{T1,T2}" /> instead.
        /// </summary>
        public Action<T1, T2> Invoke<T1, T2>(T1 arg1, T2 arg2) => Arg.Invoke(arg1, arg2);

        /// <summary>
        /// Invoke any <see cref="Action&lt;T1,T2,T3&gt;"/> argument with specified arguments whenever a matching call is made to the substitute.
        /// This is provided for compatibility with older compilers --
        /// if possible use <see cref="Arg.Invoke{T1,T2,T3}" /> instead.
        /// </summary>
        public Action<T1, T2, T3> Invoke<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3) => Arg.Invoke(arg1, arg2, arg3);

        /// <summary>
        /// Invoke any <see cref="Action&lt;T1,T2,T3,T4&gt;"/> argument with specified arguments whenever a matching call is made to the substitute.
        /// This is provided for compatibility with older compilers --
        /// if possible use <see cref="Arg.Invoke{T1,T2,T3,T4}" /> instead.
        /// </summary>
        public Action<T1, T2, T3, T4> Invoke<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => Arg.Invoke(arg1, arg2, arg3, arg4);

        /// <summary>
        /// Invoke any <typeparamref name="TDelegate"/> argument with specified arguments whenever a matching call is made to the substitute.
        /// This is provided for compatibility with older compilers --
        /// if possible use <see cref="Arg.InvokeDelegate{TDelegate}" /> instead.
        /// </summary>
        /// <param name="arguments">Arguments to pass to delegate.</param>
        public TDelegate InvokeDelegate<TDelegate>(params object[] arguments) => Arg.InvokeDelegate<TDelegate>(arguments);

        /// <summary>
        /// Capture any argument compatible with type <typeparamref name="T"/> and use it to call the <paramref name="useArgument"/> function 
        /// whenever a matching call is made to the substitute.
        /// This is provided for compatibility with older compilers --
        /// if possible use <see cref="Arg.Do{T}" /> instead.
        /// </summary>
        public T Do<T>(Action<T> useArgument) => Arg.Do(useArgument);
    }
}
