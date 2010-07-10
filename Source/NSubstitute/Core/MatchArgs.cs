namespace NSubstitute.Core
{
    public class MatchArgs
    {
        private MatchArgs() { }

        public static MatchArgs AsSpecifiedInCall = new MatchArgs();
        public static MatchArgs Any = new MatchArgs();
    }
}