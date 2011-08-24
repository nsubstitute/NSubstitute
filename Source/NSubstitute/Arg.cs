using System;
using System.Linq.Expressions;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;

namespace NSubstitute
{
    public static class Arg
    {
        public static T Any<T>()
        {
            return EnqueueArgumentSpec<T>(new ArgumentIsAnythingSpecification(typeof(T)));
        }

        public static T Is<T>(T value)
        {
            return EnqueueArgumentSpec<T>(new ArgumentEqualsSpecification(value, typeof(T)));
        }

        public static T Is<T>(Expression<Predicate<T>> predicate)
        {
            return EnqueueArgumentSpec<T>(new ArgumentMatchesSpecification<T>(predicate));
        }

        public static Action Invoke()
        {
            return EnqueueArgumentSpec<Action>(new ArgumentIsAnythingSpecification(typeof(Action)) { Action = InvokeDelegateAction() });
        }

        public static Action<T> Invoke<T>(T arg)
        {
            return EnqueueArgumentSpec<Action<T>>(new ArgumentIsAnythingSpecification(typeof(Action<T>)) { Action = InvokeDelegateAction(arg) });
        }

        public static Action<T1, T2> Invoke<T1, T2>(T1 arg1, T2 arg2)
        {
            return EnqueueArgumentSpec<Action<T1, T2>>(new ArgumentIsAnythingSpecification(typeof(Action<T1, T2>)) { Action = InvokeDelegateAction(arg1, arg2) });
        }

        public static Action<T1, T2, T3> Invoke<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            return EnqueueArgumentSpec<Action<T1, T2, T3>>(new ArgumentIsAnythingSpecification(typeof(Action<T1, T2, T3>)) { Action = InvokeDelegateAction(arg1, arg2, arg3) });
        }

        public static Action<T1, T2, T3, T4> Invoke<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return EnqueueArgumentSpec<Action<T1, T2, T3, T4>>(new ArgumentIsAnythingSpecification(typeof(Action<T1, T2, T3, T4>)) { Action = InvokeDelegateAction(arg1, arg2, arg3, arg3) });
        }

        public static TDelegate InvokeDelegate<TDelegate>(params object[] arguments)
        {
            return EnqueueArgumentSpec<TDelegate>(new ArgumentIsAnythingSpecification(typeof(TDelegate)) { Action = InvokeDelegateAction(arguments) });
        }

        public static T Use<T>(Action<T> useArgument)
        {
            return EnqueueArgumentSpec<T>(new ArgumentIsAnythingSpecification(typeof(T)) { Action = x => useArgument((T) x) });
        }

        private static T EnqueueArgumentSpec<T>(IArgumentSpecification argumentSpecification)
        {
            SubstitutionContext.Current.EnqueueArgumentSpecification(argumentSpecification);
            return default(T);
        }

        private static Action<object> InvokeDelegateAction(params object[] arguments)
        {
            return x => ((Delegate)x).DynamicInvoke(arguments);

        }
    }
}
