using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    [Serializable]
    public class CanNotPartiallySubForInterfaceOrDelegateException : SubstituteException
    {
        public CanNotPartiallySubForInterfaceOrDelegateException(Type type) : base(DescribeProblem(type)) { }
        protected CanNotPartiallySubForInterfaceOrDelegateException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        private static string DescribeProblem(Type type)
        {
            return string.Format("Can only substitute for parts of classes, not interfaces or delegates. "
                                + "Try `Substitute.For<{0}> instead.", type.Name);
        }
    }
}
