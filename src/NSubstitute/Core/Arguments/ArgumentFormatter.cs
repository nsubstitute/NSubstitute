using System;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentFormatter : IArgumentFormatter
    {
        public string Format(object argument, bool highlight)
        {
            var formatted = Format(argument);
            return highlight ? "*" + formatted + "*" : formatted;
        }

        private string Format(object arg)
        {
            if (arg == null) return "<null>";
            if (arg is string) return string.Format("\"{0}\"", arg);
            var standardToString = arg.ToString();
            if (standardToString == arg.GetType().ToString()) return FormatType(arg.GetType());
            return standardToString;
       }

        private string FormatType(Type type)
        {
            var typeName = type.Name;
            if (!type.IsGenericType()) return typeName;

            typeName = typeName.Substring(0, typeName.IndexOf('`'));
            var genericArgTypes = type.GetGenericArguments().Select(x => FormatType(x));
            return string.Format("{0}<{1}>", typeName, string.Join(", ", genericArgTypes.ToArray()));
        }
    }
}