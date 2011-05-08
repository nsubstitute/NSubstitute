using System;
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
        }

        public object[] Args()
        {
            return _callArguments.Select(x => x.Value).ToArray();
        }

        public T Arg<T>()
        {
            var type = typeof(T);
            var matchingArgs = _callArguments.Where(x => x.DeclaredType == type);
            var numberOfMatchingArgs = matchingArgs.Count();
            if (numberOfMatchingArgs > 1)
            {
                throw new AmbiguousArgumentsException("There is more than one argument of type " + typeof (T).FullName + " to this call.");
            }
            if (numberOfMatchingArgs == 0)
            {
                throw new ArgumentNotFoundException("Can not find an argument of type " + typeof (T).FullName + " to this call.");
            }
            return (T) matchingArgs.First().Value;
        }

        public Type[] ArgTypes()
        {
            return _callArguments.Select(x => x.DeclaredType).ToArray();
        }
    }
}