using System;
using System.Globalization;
using System.Reflection;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentFormatter : IArgumentFormatter
    {
        internal static IArgumentFormatter Default { get; } = new ArgumentFormatter();

        public string Format(object? argument, bool highlight)
        {
            var formatted = Format(argument);
            return highlight ? "*" + formatted + "*" : formatted;
        }

        private string Format(object? arg)
        {
            switch (arg)
            {
                case null:
                    return "<null>";

                case string str:
                    return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", str);

                case object obj when obj.GetType().GetMethod(nameof(ToString), Type.EmptyTypes)!.DeclaringType == typeof(object):
                    return arg.GetType().GetNonMangledTypeName();

                default:
                    return arg.ToString() ?? string.Empty;
            }
       }
    }
}