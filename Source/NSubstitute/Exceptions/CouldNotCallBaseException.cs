using System.Reflection;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class CouldNotCallBaseException : SubstituteException
    {
        public CouldNotCallBaseException(MethodInfo methodInfo) : base(DescribeProblem(methodInfo)) { }
        protected CouldNotCallBaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        private static string DescribeProblem(MethodInfo member)
        {
            // todo add more descriptive explanation of the exception
            return string.Format("Can not call base implementation for {0}.{1}.", member.DeclaringType.Name, member.Name);
        }

    }
}