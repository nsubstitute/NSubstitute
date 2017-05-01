namespace NSubstitute.Core
{
    public class MatchArgs
    {
        private MatchArgs() { }

        public static readonly MatchArgs AsSpecifiedInCall = new MatchArgs();
        public static readonly MatchArgs Any = new MatchArgs();
    }
}