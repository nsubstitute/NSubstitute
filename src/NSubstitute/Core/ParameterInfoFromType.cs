using System;

namespace NSubstitute.Core
{
    class ParameterInfoFromType : IParameterInfo
    {
        private readonly Type _parameterType;

        public ParameterInfoFromType(Type parameterType)
        {
            _parameterType = parameterType;
        }

        public Type ParameterType
        {
            get { return _parameterType; }
        }

        public bool IsParams
        {
            get { return false; }
        }

        public bool IsOptional
        {
            get { return false; }
        }

        public bool IsOut
        {
            get { return false; }
        }
    }
}