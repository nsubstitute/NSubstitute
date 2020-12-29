using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class Call : ICall, /* Performance optimization */ CallCollection.IReceivedCallEntry
    {
        private readonly MethodInfo _methodInfo;
        private readonly object?[] _arguments;
        private object?[] _originalArguments;
        private readonly object _target;
        private readonly IList<IArgumentSpecification> _argumentSpecifications;
        private IParameterInfo[]? _parameterInfosCached;
        private long? _sequenceNumber;
        private readonly Func<object>? _baseMethod;

        [Obsolete("This constructor is deprecated and will be removed in future version of product.")]
        public Call(MethodInfo methodInfo,
            object?[] arguments,
            object target,
            IList<IArgumentSpecification> argumentSpecifications,
            IParameterInfo[] parameterInfos,
            Func<object> baseMethod)
            : this(methodInfo, arguments, target, argumentSpecifications, baseMethod)
        {
            _parameterInfosCached = parameterInfos ?? throw new ArgumentNullException(nameof(parameterInfos));
        }

        public Call(MethodInfo methodInfo,
            object?[] arguments,
            object target,
            IList<IArgumentSpecification> argumentSpecifications,
            Func<object>? baseMethod)
        {
            _methodInfo = methodInfo;
            _arguments = arguments;
            _target = target;
            _argumentSpecifications = argumentSpecifications;
            _baseMethod = baseMethod;

            // Performance optimization - we don't want to create a copy on each call.
            // Instead, we want to guard array only if "mutable" array property is accessed.
            // Therefore, we keep tracking whether the "mutable" version is accessed and if so - create a copy on demand.
            _originalArguments = _arguments;
        }

        public IParameterInfo[] GetParameterInfos()
        {
            // Don't worry about concurrency.
            // Normally call is processed in a single thread.
            // However even if race condition happens, we'll create an extra set of wrappers, which behaves the same.
            return _parameterInfosCached ??= GetParameterInfoFromMethod(_methodInfo);
        }

        public IList<IArgumentSpecification> GetArgumentSpecifications() => _argumentSpecifications;

        public void AssignSequenceNumber(long number)
        {
            _sequenceNumber = number;
        }

        public long GetSequenceNumber() => _sequenceNumber ?? throw new MissingSequenceNumberException();

        public bool CanCallBase => _baseMethod != null;

        public Maybe<object?> TryCallBase()
        {
            return _baseMethod == null ? Maybe.Nothing<object?>() : Maybe.Just<object?>(_baseMethod());
        }

        public Type GetReturnType() => _methodInfo.ReturnType;

        public MethodInfo GetMethodInfo() => _methodInfo;

        public object?[] GetArguments()
        {
            // This method assumes that result might be mutated.
            // Therefore, we should guard our array with original values to ensure it's unmodified.
            // Also if array is empty - no sense to make a copy.
            object?[] originalArray = _originalArguments;
            if (originalArray == _arguments && originalArray.Length > 0)
            {
                object?[] copy = originalArray.ToArray();
                // If it happens that _originalArguments doesn't point to the `_arguments` anymore -
                // it might happen that other thread already created a copy and mutated the original `_arguments` array.
                // In this case it's unsafe to replace it with a copy.
                Interlocked.CompareExchange(ref _originalArguments, copy, originalArray);
            }

            return _arguments;
        }

        public object?[] GetOriginalArguments() => _originalArguments;

        public object Target() => _target;

        private static IParameterInfo[] GetParameterInfoFromMethod(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            var parameterInfos = new IParameterInfo[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                parameterInfos[i] = new ParameterInfoWrapper(parameters[i]);
            }

            return parameterInfos;
        }

        /* Performance optimization.
           Allows Call to carry additional information, required by received calls registry. */

        // 0 - not owned, default; 1 - owned; -1 - skipped. Use int because of Interlocked compatibility.
        private int _callEntryState;
        ICall CallCollection.IReceivedCallEntry.Call => this;
        bool CallCollection.IReceivedCallEntry.IsSkipped => _callEntryState == -1;
        void CallCollection.IReceivedCallEntry.Skip() => _callEntryState = -1;

        bool CallCollection.IReceivedCallEntry.TryTakeEntryOwnership()
        {
            return Interlocked.CompareExchange(ref _callEntryState, 1, 0) == 0;
        }
    }
}
