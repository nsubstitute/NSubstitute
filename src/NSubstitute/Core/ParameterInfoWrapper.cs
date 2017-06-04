using System;
using System.Reflection;

namespace NSubstitute.Core
{
    class ParameterInfoWrapper : IParameterInfo
    {
        private readonly ParameterInfo _parameterInfo;

        public ParameterInfoWrapper(ParameterInfo parameterInfo)
        {
            _parameterInfo = parameterInfo;
        }

        public Type ParameterType
        {
            get { return _parameterInfo.ParameterType; }
        }

        public bool IsParams
        {
            get { return _parameterInfo.IsParams(); }
        }

        public bool IsOptional
        {
            get { return _parameterInfo.IsOptional; }
        }

        public bool IsOut
        {
            get { return _parameterInfo.IsOut; }
        }
    }
}