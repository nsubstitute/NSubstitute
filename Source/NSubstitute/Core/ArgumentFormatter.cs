using System;

namespace NSubstitute.Core
{
    public class ArgumentFormatter : IArgumentFormatter
    {
        public string Format(object arg)
        {
            if (arg == null) return "<null>";
            if (arg is string) return string.Format("\"{0}\"", arg);
            return arg.ToString();
       }
    }
}