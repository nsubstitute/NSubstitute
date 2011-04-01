using System;
using System.Linq;
using NSubstitute.Exceptions;

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
            set { _callArguments[index] = value; }
        }

        public object[] Args()
        {
            return _callArguments;
        }

        public T Arg<T>()
        {
            var matchingArgs = _callArguments.Where(x => x is T);
            var numberOfMatchingArgs = matchingArgs.Count();
            if (numberOfMatchingArgs > 1)
            {
                throw new AmbiguousArgumentsException("There is more than one argument of type " + typeof (T).FullName +
                                                      " to this call.");
            }
            if (numberOfMatchingArgs == 0)
            {
                throw new ArgumentNotFoundException("Can not find an argument of type " + typeof (T).FullName +
                                                    " to this call.");
            }
            return (T) matchingArgs.First();
        }
    }
}