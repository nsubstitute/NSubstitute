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

        public Call(MethodInfo methodInfo, object[] arguments, object target, IList<IArgumentSpecification> argumentSpecsForCall, Func<object> baseMethod = null) 
        {
            _methodInfo = methodInfo;
            _arguments = arguments;
            _originalArguments = arguments.ToArray();
            _target = target;
            _parameterInfos = GetParameterInfosFrom(_methodInfo);
            _argumentSpecifications = argumentSpecsForCall;
            _baseMethod = baseMethod;
        }

        public Call(MethodInfo methodInfo, object[] arguments, object target, IParameterInfo[] parameterInfos)
        {
            _methodInfo = methodInfo;
            _arguments = arguments;
            _originalArguments = arguments.ToArray();
            _target = target;
            _parameterInfos = parameterInfos ?? GetParameterInfosFrom(_methodInfo);
            _argumentSpecifications = (_parameterInfos.Length == 0) ? EmptyList() : SubstitutionContext.Current.DequeueAllArgumentSpecifications();
        }

        private IList<IArgumentSpecification> EmptyList()
        {
            return new List<IArgumentSpecification>();
        }

        private IParameterInfo[] GetParameterInfosFrom(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters == null) return new IParameterInfo[0];
            return parameters.Select(x => new ParameterInfoWrapper(x)).ToArray();
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
