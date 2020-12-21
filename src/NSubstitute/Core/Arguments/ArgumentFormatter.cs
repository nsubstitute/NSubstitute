using System;
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
            return arg switch
            {
                null                                 => "<null>",
                string str                           => $"\"{str}\"",
                { } obj when HasDefaultToString(obj) => arg.GetType().GetNonMangledTypeName(),
                _                                    => arg.ToString() ?? string.Empty
            };

            static bool HasDefaultToString(object obj)
                => obj.GetType().GetMethod(nameof(ToString), Type.EmptyTypes)!.DeclaringType == typeof(object);
        }
    }
}