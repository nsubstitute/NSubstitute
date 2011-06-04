using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class CallInfo
    {
        private readonly Argument[] _callArguments;

        public CallInfo(Argument[] callArguments)
        {
            _callArguments = callArguments;
        }

        public object this[int index]
        {
            get { return _callArguments[index].Value; }
            set
            {
                var argument = _callArguments[index];
                EnsureArgIsSettable(argument, index, value); 
                argument.Value = value;
            }
        }

        private void EnsureArgIsSettable(Argument argument, int index, object value)
        {
            if (!argument.IsByRef)
            {
                throw new ArgumentIsNotOutOrRefException(index, argument.DeclaredType);
            }

            if (value != null && !argument.CanSetValueWithInstanceOf(value.GetType()))
            {
                throw new ArgumentSetWithIncompatibleValueException(index, argument.DeclaredType, value.GetType());
            }
        }

        public object[] Args()
        {
            return _callArguments.Select(x => x.Value).ToArray();
        }

        public Type[] ArgTypes()
        {
            return _callArguments.Select(x => x.DeclaredType).ToArray();
        }

        public T Arg<T>()
        {
            T arg;
            if (TryGetArg(x => x.IsDeclaredTypeEqualToOrByRefVersionOf(typeof(T)), out arg)) return arg;
            if (TryGetArg(x => x.IsValueAssignableTo(typeof(T)), out arg)) return arg;
            throw new ArgumentNotFoundException("Can not find an argument of type " + typeof(T).FullName + " to this call.");
        }

        private bool TryGetArg<T>(Func<Argument, bool> condition, out T value)
        {
            value = default(T);

            var matchingArgs = _callArguments.Where(condition);
            if (!matchingArgs.Any()) return false;
            ThrowIfMoreThanOne<T>(matchingArgs);

            value = (T)matchingArgs.First().Value;
            return true;
        }

        private void ThrowIfMoreThanOne<T>(IEnumerable<Argument> arguments)
        {
            if (arguments.Skip(1).Any())
            {
                throw new AmbiguousArgumentsException(
                    "There is more than one argument of type " + typeof(T).FullName + " to this call.\n" +
                    "The call signature is (" + DisplayTypes(ArgTypes()) + ")\n" +
                    "  and was called with (" + DisplayTypes(_callArguments.Select(x => x.ActualType)) + ")"
                    );
            }
        }

        private static string DisplayTypes(IEnumerable<Type> types)
        {
            return string.Join(", ", types.Select(x => x.Name).ToArray());
        }
    }
}