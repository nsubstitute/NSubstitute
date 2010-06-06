using System;
using System.Reflection;

namespace NSubstitute.Core
{
    public class CallFormatter : ICallFormatter
    {
        public string Format(MethodInfo methodInfoOfCall)
        {
            return methodInfoOfCall.Name;
        }
    }
}