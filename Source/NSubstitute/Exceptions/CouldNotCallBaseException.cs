using System.Reflection;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class CouldNotCallBaseException : SubstituteException
    {
        private const string WhatProbablyWentWrong = "Make sure you substituted for a class rather than abstraction (interface, abstract class etc).";

        public CouldNotCallBaseException(MethodInfo methodInfo) : base(DescribeProblem(methodInfo) + "\n\n" + WhatProbablyWentWrong) { }
        protected CouldNotCallBaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        private static string DescribeProblem(MethodInfo member)
        {
            return string.Format("Can not call base implementation for {0}.{1}.", member.DeclaringType.Name, member.Name);
        }

    }
}