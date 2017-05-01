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
        private readonly static ArgumentSpecificationQueue ArgSpecQueue = new ArgumentSpecificationQueue(SubstitutionContext.Current);

        /// <summary>
        /// Match any argument value compatible with type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Any<T>()
        {
            return ArgSpecQueue.EnqueueSpecFor<T>(new AnyArgumentMatcher(typeof(T)));
        }

        /// <summary>
        /// Match argument that is equal to <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Is<T>(T value)
        {
            return ArgSpecQueue.EnqueueSpecFor<T>(new EqualsArgumentMatcher(value));
        }

        /// <summary>
        /// Match argument that satisfies <paramref name="predicate"/>. 
        /// If the <paramref name="predicate"/> throws an exception for an argument it will be treated as non-matching.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T Is<T>(Expression<Predicate<T>> predicate)
        {
            return ArgSpecQueue.EnqueueSpecFor<T>(new ExpressionArgumentMatcher<T>(predicate));
        }

        /// <summary>
        /// Invoke any <see cref="Action"/> argument whenever a matching call is made to the substitute.
        /// </summary>
        /// <returns></returns>
        public static Action Invoke()
        {
            return ArgSpecQueue.EnqueueSpecFor<Action>(new AnyArgumentMatcher(typeof(Action)), InvokeDelegateAction());
        }

        /// <summary>
        /// Invoke any <see cref="Action&lt;T&gt;"/> argument with specified argument whenever a matching call is made to the substitute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static Action<T> Invoke<T>(T arg)
        {
            return ArgSpecQueue.EnqueueSpecFor<Action<T>>(new AnyArgumentMatcher(typeof(Action<T>)), InvokeDelegateAction(arg));
        }

        /// <summary>
        /// Invoke any <see cref="Action&lt;T1,T2&gt;"/> argument with specified arguments whenever a matching call is made to the substitute.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static Action<T1, T2> Invoke<T1, T2>(T1 arg1, T2 arg2)
        {
            return ArgSpecQueue.EnqueueSpecFor<Action<T1, T2>>(new AnyArgumentMatcher(typeof(Action<T1, T2>)), InvokeDelegateAction(arg1, arg2));
        }

        /// <summary>
        /// Invoke any <see cref="Action&lt;T1,T2,T3&gt;"/> argument with specified arguments whenever a matching call is made to the substitute.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        public static Action<T1, T2, T3> Invoke<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            return ArgSpecQueue.EnqueueSpecFor<Action<T1, T2, T3>>(new AnyArgumentMatcher(typeof(Action<T1, T2, T3>)), InvokeDelegateAction(arg1, arg2, arg3));
        }

        /// <summary>
        /// Invoke any <see cref="Action&lt;T1,T2,T3,T4&gt;"/> argument with specified arguments whenever a matching call is made to the substitute.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <returns></returns>
        public static Action<T1, T2, T3, T4> Invoke<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return ArgSpecQueue.EnqueueSpecFor<Action<T1, T2, T3, T4>>(new AnyArgumentMatcher(typeof(Action<T1, T2, T3, T4>)), InvokeDelegateAction(arg1, arg2, arg3, arg4));
        }

        /// <summary>
        /// Invoke any <typeparamref name="TDelegate"/> argument with specified arguments whenever a matching call is made to the substitute.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="arguments">Arguments to pass to delegate.</param>
        /// <returns></returns>
        public static TDelegate InvokeDelegate<TDelegate>(params object[] arguments)
        {
            return ArgSpecQueue.EnqueueSpecFor<TDelegate>(new AnyArgumentMatcher(typeof(TDelegate)), InvokeDelegateAction(arguments));
        }

        /// <summary>
        /// Capture any argument compatible with type <typeparamref name="T"/> and use it to call the <paramref name="useArgument"/> function 
        /// whenever a matching call is made to the substitute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="useArgument"></param>
        /// <returns></returns>
        public static T Do<T>(Action<T> useArgument)
        {
            return ArgSpecQueue.EnqueueSpecFor<T>(new AnyArgumentMatcher(typeof(T)), x => useArgument((T)x));
        }

        private static Action<object> InvokeDelegateAction(params object[] arguments)
        {
            return x => ((Delegate)x).DynamicInvoke(arguments);
        }
    }
}
