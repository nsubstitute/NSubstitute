using System;
using System.Linq.Expressions;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;

namespace NSubstitute
{
    /// <summary>
    /// Argument matchers used for specifying calls to substitutes.
    /// </summary>
    public static class Arg
    {
        /// <summary>
        /// Match any argument value compatible with type <typeparamref name="T"/>.
        /// </summary>
        public static ref T Any<T>()
        {
            return ref EnqueueSpecFor<T>(new AnyArgumentMatcher(typeof(T)));
        }

        /// <summary>
        /// Match argument that is equal to <paramref name="value"/>.
        /// </summary>
        public static ref T Is<T>(T value)
        {
            return ref EnqueueSpecFor<T>(new EqualsArgumentMatcher(value));
        }

        /// <summary>
        /// Match argument that satisfies <paramref name="predicate"/>. 
        /// If the <paramref name="predicate"/> throws an exception for an argument it will be treated as non-matching.
        /// </summary>
        public static ref T Is<T>(Expression<Predicate<T>> predicate)
        {
            return ref EnqueueSpecFor<T>(new ExpressionArgumentMatcher<T>(predicate));
        }

        /// <summary>
        /// Invoke any <see cref="Action"/> argument whenever a matching call is made to the substitute.
        /// </summary>
        public static ref Action Invoke()
        {
            return ref EnqueueSpecFor<Action>(new AnyArgumentMatcher(typeof(Action)), InvokeDelegateAction());
        }

        /// <summary>
        /// Invoke any <see cref="Action&lt;T&gt;"/> argument with specified argument whenever a matching call is made to the substitute.
        /// </summary>
        public static ref Action<T> Invoke<T>(T arg)
        {
            return ref EnqueueSpecFor<Action<T>>(new AnyArgumentMatcher(typeof(Action<T>)), InvokeDelegateAction(arg));
        }

        /// <summary>
        /// Invoke any <see cref="Action&lt;T1,T2&gt;"/> argument with specified arguments whenever a matching call is made to the substitute.
        /// </summary>
        public static ref Action<T1, T2> Invoke<T1, T2>(T1 arg1, T2 arg2)
        {
            return ref EnqueueSpecFor<Action<T1, T2>>(new AnyArgumentMatcher(typeof(Action<T1, T2>)), InvokeDelegateAction(arg1, arg2));
        }

        /// <summary>
        /// Invoke any <see cref="Action&lt;T1,T2,T3&gt;"/> argument with specified arguments whenever a matching call is made to the substitute.
        /// </summary>
        public static ref Action<T1, T2, T3> Invoke<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            return ref EnqueueSpecFor<Action<T1, T2, T3>>(new AnyArgumentMatcher(typeof(Action<T1, T2, T3>)), InvokeDelegateAction(arg1, arg2, arg3));
        }

        /// <summary>
        /// Invoke any <see cref="Action&lt;T1,T2,T3,T4&gt;"/> argument with specified arguments whenever a matching call is made to the substitute.
        /// </summary>
        public static ref Action<T1, T2, T3, T4> Invoke<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return ref EnqueueSpecFor<Action<T1, T2, T3, T4>>(new AnyArgumentMatcher(typeof(Action<T1, T2, T3, T4>)), InvokeDelegateAction(arg1, arg2, arg3, arg4));
        }

        /// <summary>
        /// Invoke any <typeparamref name="TDelegate"/> argument with specified arguments whenever a matching call is made to the substitute.
        /// </summary>
        /// <param name="arguments">Arguments to pass to delegate.</param>
        public static ref TDelegate InvokeDelegate<TDelegate>(params object[] arguments)
        {
            return ref EnqueueSpecFor<TDelegate>(new AnyArgumentMatcher(typeof(TDelegate)), InvokeDelegateAction(arguments));
        }

        /// <summary>
        /// Capture any argument compatible with type <typeparamref name="T"/> and use it to call the <paramref name="useArgument"/> function 
        /// whenever a matching call is made to the substitute.
        /// </summary>
        public static ref T Do<T>(Action<T> useArgument)
        {
            return ref EnqueueSpecFor<T>(new AnyArgumentMatcher(typeof(T)), x => useArgument((T)x));
        }

        private static ref T EnqueueSpecFor<T>(IArgumentMatcher argumentMatcher)
        {
            var argumentSpecification = new ArgumentSpecification(typeof(T), argumentMatcher);
            return ref EnqueueArgSpecification<T>(argumentSpecification);
        }

        private static ref T EnqueueSpecFor<T>(IArgumentMatcher argumentMatcher, Action<object> action)
        {
            var argumentSpecification = new ArgumentSpecification(typeof(T), argumentMatcher, action);
            return ref EnqueueArgSpecification<T>(argumentSpecification);
        }

        private static ref T EnqueueArgSpecification<T>(IArgumentSpecification specification)
        {
            SubstitutionContext.Current.ThreadContext.EnqueueArgumentSpecification(specification);
            return ref new DefaultValueContainer<T>().Value;
        }

        private static Action<object> InvokeDelegateAction(params object[] arguments)
        {
            return x => ((Delegate)x).DynamicInvoke(arguments);
        }

        private class DefaultValueContainer<T>
        {
            public T Value;
        }
    }
}
