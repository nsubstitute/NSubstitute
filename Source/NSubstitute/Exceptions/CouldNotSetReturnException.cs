using System;
using System.Reflection;
#if NET35 || NET4 || NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public abstract class CouldNotSetReturnException : SubstituteException
    {
        protected const string WhatProbablyWentWrong = 
                "Make sure you called Returns() after calling your substitute (for example: mySub.SomeMethod().Returns(value)),\n" +
                "and that you are not configuring other substitutes within Returns() (for example, avoid this: mySub.SomeMethod().Returns(ConfigOtherSub())).\n" +
                "\n" +
                "If you substituted for a class rather than an interface, check that the call to your substitute was on a virtual/abstract member.\n" +
                "Return values cannot be configured for non-virtual/non-abstract members.\n" +
                "\n" +
                "Correct use:\n" +
                "\tmySub.SomeMethod().Returns(returnValue);\n" +
                "\n" +
                "Potentially problematic use:\n" +
                "\tmySub.SomeMethod().Returns(ConfigOtherSub());\n" +
                "Instead try:\n" +
                "\tvar returnValue = ConfigOtherSub();\n" +
                "\tmySub.SomeMethod().Returns(returnValue);\n" +
                "";

        protected CouldNotSetReturnException(string s) : base(s + "\n\n" + WhatProbablyWentWrong) { }
#if NET35 || NET4 || NET45
        protected CouldNotSetReturnException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }

#if NET35 || NET4 || NET45
    [Serializable]
#endif
    public class CouldNotSetReturnDueToNoLastCallException : CouldNotSetReturnException
    {
        public CouldNotSetReturnDueToNoLastCallException() : base("Could not find a call to return from.") { }
#if NET35 || NET4 || NET45
        protected CouldNotSetReturnDueToNoLastCallException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }

#if NET35 || NET4 || NET45
    [Serializable]
#endif
    public class CouldNotSetReturnDueToTypeMismatchException : CouldNotSetReturnException
    {
        public CouldNotSetReturnDueToTypeMismatchException(Type returnType, MethodInfo member) : base(DescribeProblem(returnType, member)) { }

        private static string DescribeProblem(Type typeOfReturnValueOrNull, MethodInfo member)
        {
            return typeOfReturnValueOrNull == null 
                ? String.Format("Can not return null for {0}.{1} (expected type {2}).", member.DeclaringType.Name, member.Name, member.ReturnType.Name) 
                : String.Format("Can not return value of type {0} for {1}.{2} (expected type {3}).", typeOfReturnValueOrNull.Name, member.DeclaringType.Name, member.Name, member.ReturnType.Name);
        }

#if NET35 || NET4 || NET45
        protected CouldNotSetReturnDueToTypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
