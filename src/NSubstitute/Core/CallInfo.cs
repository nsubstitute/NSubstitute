using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute.Exceptions;

// Disable nullability for entry-point API
#nullable disable annotations

namespace NSubstitute.Core
{
    public class CallInfo : ICallInfo
    {
        private readonly Argument[] _callArguments;
        private readonly Func<Maybe<object>> _baseResult;

        public CallInfo(Argument[] callArguments, Func<Maybe<object>> baseResult) {
            _callArguments = callArguments;
            _baseResult = baseResult;
        }

        protected CallInfo(CallInfo info) : this(info._callArguments, info._baseResult) {
        }

        /// <summary>
        /// Call and returns the result from the base implementation of a substitute for a class.
        /// Will throw an exception if no base implementation exists.
        /// </summary>
        /// <returns>Result from base implementation</returns>
        /// <exception cref="NoBaseImplementationException">Throws in no base implementation exists</exception>
        protected object GetBaseResult() {
            return _baseResult().ValueOr(() => throw new NoBaseImplementationException());
        }

        /// <inheritdoc/>
        public object this[int index] {
            get => _callArguments[index].Value;
            set {
                var argument = _callArguments[index];
                EnsureArgIsSettable(argument, index, value);
                argument.Value = value;
            }
        }

        private void EnsureArgIsSettable(Argument argument, int index, object value) {
            if (!argument.IsByRef) {
                throw new ArgumentIsNotOutOrRefException(index, argument.DeclaredType);
            }

            if (value != null && !argument.CanSetValueWithInstanceOf(value.GetType())) {
                throw new ArgumentSetWithIncompatibleValueException(index, argument.DeclaredType, value.GetType());
            }
        }

        /// <inheritdoc/>
        public object[] Args() => _callArguments.Select(x => x.Value).ToArray();

        /// <inheritdoc/>
        public Type[] ArgTypes() => _callArguments.Select(x => x.DeclaredType).ToArray();

        /// <inheritdoc/>
        public T Arg<T>() {
            T arg;
            if (TryGetArg(x => x.IsDeclaredTypeEqualToOrByRefVersionOf(typeof(T)), out arg)) return arg;
            if (TryGetArg(x => x.IsValueAssignableTo(typeof(T)), out arg)) return arg;
            throw new ArgumentNotFoundException("Can not find an argument of type " + typeof(T).FullName + " to this call.");
        }

        private bool TryGetArg<T>(Func<Argument, bool> condition, [MaybeNullWhen(false)] out T value) {
            value = default;

            var matchingArgs = _callArguments.Where(condition);
            if (!matchingArgs.Any()) return false;
            ThrowIfMoreThanOne<T>(matchingArgs);

            value = (T)matchingArgs.First().Value!;
            return true;
        }

        private void ThrowIfMoreThanOne<T>(IEnumerable<Argument> arguments) {
            if (arguments.Skip(1).Any()) {
                throw new AmbiguousArgumentsException(
                    "There is more than one argument of type " + typeof(T).FullName + " to this call.\n" +
                    "The call signature is (" + DisplayTypes(ArgTypes()) + ")\n" +
                    "  and was called with (" + DisplayTypes(_callArguments.Select(x => x.ActualType)) + ")"
                    );
            }
        }

        /// <inheritdoc/>
        public T ArgAt<T>(int position) {
            if (position >= _callArguments.Length) {
                throw new ArgumentOutOfRangeException(nameof(position), $"There is no argument at position {position}");
            }

            try {
                return (T)_callArguments[position].Value!;
            } catch (InvalidCastException) {
                throw new InvalidCastException(
                    $"Couldn't convert parameter at position {position} to type {typeof(T).FullName}");
            }
        }

        private static string DisplayTypes(IEnumerable<Type> types) =>
            string.Join(", ", types.Select(x => x.Name).ToArray());

        /// <inheritdoc/>
        public ICallInfo<T> ForCallReturning<T>() => new CallInfo<T>(this);
    }
}
