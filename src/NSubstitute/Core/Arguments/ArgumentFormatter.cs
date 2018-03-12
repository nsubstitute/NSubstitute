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
            if (standardToString == arg.GetType().ToString()) return arg.GetType().GetNonMangledTypeName();
            return standardToString;
       }
    }
}