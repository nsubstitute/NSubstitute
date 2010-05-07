using System;

namespace NSubstitute.Core
{
    public class CallInfo
    {
        private readonly object[] _callArguments;

        public CallInfo(object[] callArguments)
        {
            _callArguments = callArguments;
        }

        public object this[int index]
        {
            get { return _callArguments[index]; }
        }

        public object[] GetArguments()
        {
            return _callArguments;
        }
    }
}