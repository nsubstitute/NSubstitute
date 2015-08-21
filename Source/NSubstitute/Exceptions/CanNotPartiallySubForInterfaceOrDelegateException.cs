using System;
#if NET4 || NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public class CanNotPartiallySubForInterfaceOrDelegateException : SubstituteException
    {
        public CanNotPartiallySubForInterfaceOrDelegateException(Type type) : base(DescribeProblem(type)) { }
#if NET4 || NET45
        protected CanNotPartiallySubForInterfaceOrDelegateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
        private static string DescribeProblem(Type type)
        {
            return string.Format("Can only substitute for parts of classes, not interfaces or delegates. "
                                + "Try `Substitute.For<{0}> instead.", type.Name);
        }
    }
}