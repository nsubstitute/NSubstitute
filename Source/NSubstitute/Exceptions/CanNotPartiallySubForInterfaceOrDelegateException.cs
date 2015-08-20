using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class CanNotPartiallySubForInterfaceOrDelegateException : SubstituteException
    {
        public CanNotPartiallySubForInterfaceOrDelegateException(Type type) : base(DescribeProblem(type)) { }

        private static string DescribeProblem(Type type)
        {
            return string.Format("Can only substitute for parts of classes, not interfaces or delegates. "
                                + "Try `Substitute.For<{0}> instead.", type.Name);
        }
    }
}