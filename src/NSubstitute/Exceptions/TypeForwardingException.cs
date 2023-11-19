using System;

namespace NSubstitute.Exceptions
{
    public abstract class TypeForwardingException : SubstituteException
    {
        protected TypeForwardingException(string message) : base(message) { }
    }

    public sealed class CanNotForwardCallsToClassNotImplementingInterfaceException : TypeForwardingException
    {
        public CanNotForwardCallsToClassNotImplementingInterfaceException(Type type) : base(DescribeProblem(type)) { }
        private static string DescribeProblem(Type type)
        {
            return string.Format("The provided class '{0}' doesn't implement all requested interfaces. ", type.Name);
        }
    }

    public sealed class CanNotForwardCallsToAbstractClassException : TypeForwardingException
    {
        public CanNotForwardCallsToAbstractClassException(Type type) : base(DescribeProblem(type)) { }
        private static string DescribeProblem(Type type)
        {
            return string.Format("The provided class '{0}' is abstract. ", type.Name);
        }
    }
}
