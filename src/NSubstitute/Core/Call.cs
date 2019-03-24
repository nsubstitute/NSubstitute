using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class Call : ICall
    {
        private readonly MethodInfo _methodInfo;
        private readonly object[] _arguments;
        private readonly object[] _originalArguments;
        private readonly object _target;
        private readonly IList<IArgumentSpecification> _argumentSpecifications;
        private IParameterInfo[] _parameterInfosCached;
        private long? _sequenceNumber;
        private readonly Func<object> _baseMethod;

        [Obsolete("This constructor is deprecated and will be removed in future version of product.")]
        public Call(MethodInfo methodInfo,
            object[] arguments,
            object target,
            IList<IArgumentSpecification> argumentSpecifications,
            IParameterInfo[] parameterInfos,
            Func<object> baseMethod)
            : this(methodInfo, arguments, target, argumentSpecifications, baseMethod)
        {
            _parameterInfosCached = parameterInfos ?? throw new ArgumentNullException(nameof(parameterInfos));
        }

        public Call(MethodInfo methodInfo,
            object[] arguments,
            object target,
            IList<IArgumentSpecification> argumentSpecifications,
            Func<object> baseMethod)
        {
            _methodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            _originalArguments = arguments.ToArray();
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _argumentSpecifications = argumentSpecifications ?? throw new ArgumentNullException(nameof(argumentSpecifications));
            _baseMethod = baseMethod;
        }

        public IParameterInfo[] GetParameterInfos()
        {
            // Don't worry about concurrency.
            // Normally call is processed in a single thread.
            // However even if race condition happens, we'll create an extra set of wrappers, which behaves the same.
            if (_parameterInfosCached == null)
            {
                _parameterInfosCached = GetParameterInfoFromMethod(_methodInfo);
            }

            return _parameterInfosCached;
        }

        public IList<IArgumentSpecification> GetArgumentSpecifications()
        {
            return _argumentSpecifications;
        }

        public void AssignSequenceNumber(long number)
        {
            _sequenceNumber = number;
        }

        public long GetSequenceNumber()
        {
            if (_sequenceNumber == null)
            {
                throw new MissingSequenceNumberException();
            }
            return _sequenceNumber.Value;
        }

        public bool CanCallBase => _baseMethod != null;

        public Maybe<object> TryCallBase()
        {
            return _baseMethod == null ? Maybe.Nothing<object>() : Maybe.Just(_baseMethod());
        }

        public Type GetReturnType() => _methodInfo.ReturnType;

        public MethodInfo GetMethodInfo() => _methodInfo;

        public object[] GetArguments()
        {
            return _arguments;
        }

        public object[] GetOriginalArguments()
        {
            return _originalArguments;
        }

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
    }
}
