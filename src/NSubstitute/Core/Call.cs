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
        private readonly IParameterInfo[] _parameterInfos;
        private readonly IList<IArgumentSpecification> _argumentSpecifications;
        private long? _sequenceNumber;
        private readonly Func<object> _baseMethod;

        public Call(MethodInfo methodInfo,
            object[] arguments,
            object target,
            IList<IArgumentSpecification> argumentSpecifications,
            IParameterInfo[] parameterInfos,
            Func<object> baseMethod)
        {
            _methodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            _originalArguments = arguments.ToArray();
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _argumentSpecifications = argumentSpecifications ?? throw new ArgumentNullException(nameof(argumentSpecifications));
            _parameterInfos = parameterInfos ?? throw new ArgumentNullException(nameof(parameterInfos));
            _baseMethod = baseMethod;
        }

        public IParameterInfo[] GetParameterInfos()
        {
            return _parameterInfos;
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

        public Type GetReturnType()
        {
            return _methodInfo.ReturnType;
        }

        public MethodInfo GetMethodInfo()
        {
            return _methodInfo;
        }

        public object[] GetArguments()
        {
            return _arguments;
        }

        public object[] GetOriginalArguments()
        {
            return _originalArguments;
        }

        public object Target()
        {
            return _target;
        }
    }
}
